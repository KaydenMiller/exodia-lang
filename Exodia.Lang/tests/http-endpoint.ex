/**
 * http-endpoint.ex  -- DESIGN EXPLORATION (aspirational; ahead of the grammar).
 *
 * A minimal-API-style HTTP endpoint showing domain-error -> HTTP-response mapping,
 * the thing ErrorOr helps with in C# but that gets awkward in complex pipelines.
 *
 * Demonstrates:
 *   - Result<T, E> for fallible service calls
 *   - a domain error ENUM matched exhaustively -> each kind maps to an HTTP status
 *   - `?` to keep the happy path flat instead of nested pipelines
 *   - an arm that does PROCESSING (logging) before returning, not just a value
 *   - constructor-injected services (compile-time DI) + handler-param binding
 *   - `match` (destructure a sum type) vs `switch` (dispatch on a plain value)
 *
 * OPEN: the exact switch-vs-match split is still being decided -- this file is the
 * hypothetical for seeing how each reads.
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
                    // an arm can DO work, not just yield a value: the block's
                    // last expression is the arm's result.
                    Logging::Error(detail);
                    Http::Status(500)
                },
            },
        };
    }

    // Contrast: `switch` dispatches on a plain VALUE (no destructuring, discrete cases),
    // where `match` above destructured a sum type and bound its payloads.
    public fn StatusText(code: int32): String {
        return switch code {
            200 => "OK",
            401 => "Unauthorized",
            404 => "Not Found",
            422 => "Unprocessable Entity",
            500 => "Server Error",
            _   => "Unknown",
        };
    }
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
