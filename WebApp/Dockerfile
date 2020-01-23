FROM golang:1.13 AS build
RUN curl -sL https://deb.nodesource.com/setup_13.x | bash - &&\
        apt-get install -y nodejs tree
WORKDIR /build
ADD . .
RUN go mod tidy
RUN go build -o ./bin/server ./cmd/server/*.go
RUN cd ./web &&\
    npm ci &&\
    npx ng build --prod &&\
    cp -r ./dist/WebApp /build/bin

FROM debian:stretch-slim AS final
WORKDIR /app
COPY --from=build /build/bin .

EXPOSE 80
ENTRYPOINT ["/app/server"]
CMD ["-dir", "/app/WebApp", "-addr", ":80", "-routes", "summoner"]
