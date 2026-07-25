// http-server.ex -- a minimal HTTP server, entirely in Exodia, over the C socket shim.
//
// Blocking, single-threaded, fixed response. Proves the whole toolchain reaches the
// network: Exodia -> LLVM IR -> llc -> clang (+ runtime/net.c) -> a real listening socket.
//
// Build + run:
//   dotnet run --project Exodia.Lang -- examples/http-server.ex \
//     | llc -relocation-model=pic -filetype=obj -o /tmp/server.o - \
//     && clang /tmp/server.o runtime/net.c -o /tmp/server \
//     && /tmp/server
//   # then, in another shell:  curl localhost:8080

extern fn ex_listen(port: int32): int32;
extern fn ex_accept(fd: int32): int32;
extern fn ex_send(fd: int32, s: cstr): void;
extern fn ex_close(fd: int32): void;

fn main(): int32 {
    const server = ex_listen(8080);

    while (true) {
        const client = ex_accept(server);
        // `Connection: close` + closing the socket lets the client read to EOF,
        // so no Content-Length arithmetic is needed for the PoC.
        ex_send(client, "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nConnection: close\r\n\r\nHello from Exodia\n");
        ex_close(client);
    }

    return 0;
}
