# -----------------------------------------------------------------
# -- BUILD WEB APPLICATION
# -----------------------------------------------------------------
FROM node:13-alpine AS build-web

WORKDIR /build

COPY src/ src/
COPY angular.json .
COPY browserlist .
COPY package-lock.json .
COPY package.json .
COPY tsconfig.app.json .
COPY tsconfig.json .
COPY tsconfig.spec.json .

RUN npm ci \
    && npx ng build --prod

# -----------------------------------------------------------------
# -- BUILD SEITEKI
# -----------------------------------------------------------------
FROM golang:1.14-alpine AS build-stk

WORKDIR /build

RUN apk add git
RUN git clone https://github.com/zekroTJA/seiteki . --depth 1
RUN go build -o seiteki cmd/seiteki/main.go

# -----------------------------------------------------------------
# -- BUILD FINAL IMAGE
# -----------------------------------------------------------------
FROM alpine:latest AS final
LABEL maintainer="zekro <contact@zekro.de>"

WORKDIR /app

COPY --from=build-web /build/dist/WebApp .
COPY --from=build-stk /build/seiteki /bin/seiteki

RUN chmod +x /bin/seiteki

ENTRYPOINT ["/bin/seiteki", "-dir", "/app"]
CMD ["-addr", ":80"]
