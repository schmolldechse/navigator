FROM denoland/deno:alpine

WORKDIR /app

USER deno

COPY . .
RUN deno install

EXPOSE 8000

CMD ["run", "--allow-all", "server.ts"]