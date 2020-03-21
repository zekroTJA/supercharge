package main

import (
	"flag"
	"log"
	"time"

	"github.com/zekroTJA/masterypointsstats/WebApp/pkg/staticserver"
)

var (
	fAddr     = flag.String("addr", "localhost:8080", "address to listen to")
	fCertFile = flag.String("cetr", "", "TLS cert file")
	fKeyFile  = flag.String("key", "", "TLS key file")
	fDir      = flag.String("dir", "./web", "Directory from which static files will be served")
	fCompress = flag.Bool("compress", false, "Enables transparent response compression if set to true")
)

func main() {
	flag.Parse()

	sslConf := &staticserver.SSLConfig{
		CertFile: *fCertFile,
		KeyFile:  *fKeyFile,
	}

	log.Printf("Server listening on %s...", *fAddr)
	server := staticserver.New(*fAddr, 30*24*time.Hour, *fCompress, *fDir, sslConf)

	server.ListenAndServe()
}
