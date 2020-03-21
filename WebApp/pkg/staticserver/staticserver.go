package staticserver

import (
	"path"
	"regexp"
	"time"

	"github.com/valyala/fasthttp"
)

// fileRx describes the regular expression to check if a request
// path is a file request or a route request
var fileRx = regexp.MustCompile(`^.*\.(ico|css|js|svg|gif|jpe?g|png)$`)

// SSLConfig contains the location string of the
// TLS key and cert file.
type SSLConfig struct {
	KeyFile  string `json:"keyfile"`
	CertFile string `json:"certfile"`
}

// StaticServer wraps around the fasthttp server and
// file FS struct.
type StaticServer struct {
	addr    string
	sslConf *SSLConfig

	fs        *fasthttp.FS
	s         *fasthttp.Server
	fsHandler fasthttp.RequestHandler
}

// New creates a new instance of StaticServer with
// passed configuration values.
func New(addr string, cacheDuration time.Duration, compress bool, staticDir string, sslConf *SSLConfig) *StaticServer {
	server := &StaticServer{
		addr:    addr,
		sslConf: sslConf,
	}

	server.s = &fasthttp.Server{
		Handler: server.requestHandler,
	}

	server.fs = &fasthttp.FS{
		CacheDuration: cacheDuration,
		Compress:      compress,
		Root:          staticDir,
		IndexNames:    []string{"index.html"},
	}

	server.fsHandler = server.fs.NewRequestHandler()

	return server
}

// ListenAndServe blocks the current go routine and
// starts the listening and serving routines.
// Depending on if the SSL config was passed properly,
// the server will be started using SSL, else, it
// will automatically use non-SSL setup.
func (server *StaticServer) ListenAndServe() error {
	useSSL := server.sslConf != nil &&
		server.sslConf.CertFile != "" &&
		server.sslConf.KeyFile != ""

	if useSSL {
		return server.s.ListenAndServeTLS(server.addr, server.sslConf.CertFile, server.sslConf.KeyFile)
	}

	return server.s.ListenAndServe(server.addr)
}

// requestHandler checks if the request destination is a
// file or a web route. If it is a file, serve the file
// via FS handler, else serve the "index.html" file.
func (server *StaticServer) requestHandler(ctx *fasthttp.RequestCtx) {
	if fileRx.Match(ctx.Path()) {
		server.fsHandler(ctx)
	} else {
		ctx.SendFile(path.Join(server.fs.Root, "index.html"))
	}
}
