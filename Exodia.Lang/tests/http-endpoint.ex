/**
 * http-endpoint.ex  -- a minimal-API-style HTTP endpoint showing domain-error ->
 * HTTP-response mapping (the thing ErrorOr helps with in C# but that gets awkward
 * in complex pipelines).
 *
 * Demonstrates:
 *   - Result<T, E> for fallible service calls
 *   - a domain error ENUM matched exhaustively -> each kind maps to an HTTP status
 *   - `?` to keep the happy path flat instead of nested pipelines
 *   - a block arm that does PROCESSING (logging) then `return`s from the function
 *   - constructor-injected services (compile-time DI) + handler-param binding
 *   - `match` for everything (no `switch`) -- it dispatches on plain values too
 */

namespace Users {

    // Domain value object (constructed elsewhere via a validating static factory).
    public struct User {
        public Id:    int64;
        public Name:  String;
        public Email: String;
    }

    // Every failure this API can surface, as one plain sum type.
    public enum ApiError {
        NotFound,
        Unauthorized,
        BadInput(String),
        Validation(String[]),
        SystemFailure(String),
    }

    // A service (entity) whose dependencies are constructor-injected by the container.
    public class UserService {
        private repo: StandardLibrary::IUserRepository;
        private log:  StandardLibrary::ILogger;

        public ctor(repo: StandardLibrary::IUserRepository, log: StandardLibrary::ILogger) {
            this.repo = repo;
            this.log  = log;
        }

        public FindById(id: int64): Result<User, ApiError> {
            if (id <= 0) {
                return Err(BadInput("id must be positive"));
            }

            // repo.Get returns Result<Option<User>, ApiError>.
            // `?` propagates a repository failure as Err early -> the happy path stays flat.
            const found = this.repo.Get(id)?;

            return match found {
                Some(user) => Ok(user),
                None       => Err(NotFound),
            };
        }
    }
}

namespace Api {

    // Minimal-API handler: `id` is bound from the route, `users` is injected by DI.
    public fn GetUser(id: int64, users: Users::UserService): Http::Response {
        const result = users.FindById(id);
        return ToResponse(result);
    }

    // The error -> HTTP mapping. This is the pipeline the whole design targets:
    // exhaustive over every ApiError kind, each mapped to a status, with room to process.
    public fn ToResponse(result: Result<Users::User, Users::ApiError>): Http::Response {
        return match result {
            Ok(user) => Http::Ok(user),
            Err(error) => match error {
                NotFound              => Http::NotFound(),
                Unauthorized          => Http::Status(401),
                BadInput(message)     => Http::BadRequest(message),
                Validation(problems)  => Http::UnprocessableEntity(problems),
                SystemFailure(detail) => {
                    // A block arm can DO work, then PRODUCE its value with `give`.
                    // `give` yields to the match (works in return AND assignment
                    // position) -- distinct from `return`, which would exit ToResponse.
                    Logging::Error(detail);
                    give Http::Status(500);
                },
            },
        };
    }

    // `match` also does plain-value dispatch (what other languages use `switch`
    // for): literal patterns + `_`. Expression-bodied, since it just yields.
    public fn StatusText(code: int32): String =>
        match code {
            200 => "OK",
            401 => "Unauthorized",
            404 => "Not Found",
            422 => "Unprocessable Entity",
            500 => "Server Error",
            _   => "Unknown",
        };
}

namespace Program {
    // Minimal-API registration: map routes to handler FUNCTIONS (first-class);
    // the container supplies each handler's injected parameters.
    global fn Main(args: String[]): int32 {
        const app = Http::WebApp::Create(args);

        app.MapGet("/users/{id}", Api::GetUser);

        app.Run();
        return 0;
    }
}
