FROM oven/bun:latest
WORKDIR /app

COPY package.json bun.lockb* ./

RUN bun install
COPY . .

ENV REDIS_HOST=navigator-redis
ENV REDIS_PORT=6379
ENV REDIS_DB=0
ENV REDIS_USER=navigator-stations
ENV REDIS_PASSWORD=password

RUN bun tsoa

EXPOSE 8000
CMD ["bun", "run", "src/server.ts"]
