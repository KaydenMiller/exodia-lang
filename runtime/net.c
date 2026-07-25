// net.c -- the thin C shim behind Exodia's socket FFI.
//
// It hides everything the language can't express yet (struct sockaddr, pointers,
// byte buffers) behind functions that take only int / C-string. Exodia `extern fn`s
// declare these; `clang server.o runtime/net.c` supplies the definitions.
//
// As the language grows real pointers/arrays/strings, functions peel off this shim
// into pure Exodia -- the boundary shrinks, it doesn't move.

#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/socket.h>

// socket + bind + listen on 0.0.0.0:<port>. Returns the listening fd, or -1 on error.
int ex_listen(int port) {
    int fd = socket(AF_INET, SOCK_STREAM, 0);
    if (fd < 0) return -1;

    int opt = 1;
    setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt));

    struct sockaddr_in addr;
    memset(&addr, 0, sizeof(addr));
    addr.sin_family      = AF_INET;
    addr.sin_addr.s_addr = INADDR_ANY;
    addr.sin_port        = htons((unsigned short)port);

    if (bind(fd, (struct sockaddr*)&addr, sizeof(addr)) < 0) return -1;
    if (listen(fd, 16) < 0) return -1;
    return fd;
}

// Block until a client connects; return the client fd.
int ex_accept(int fd) {
    return accept(fd, NULL, NULL);
}

// Write a null-terminated string to a socket (length via strlen -- no length arg needed).
void ex_send(int fd, const char* s) {
    write(fd, s, strlen(s));
}

void ex_close(int fd) {
    close(fd);
}
