FROM oven/bun:latest
WORKDIR /app

COPY package.json bun.lockb* ./

RUN bun install
COPY . .

# caching Stations in redis
ENV REDIS_HOST=navigator-redis
ENV REDIS_PORT=6379
ENV REDIS_DB=0
ENV REDIS_USER=navigator-stations
ENV REDIS_PASSWORD=password

# BetterAuth
ENV AUTH_SECRET=auth-secret
ENV AUTH_POSTGRES_URL=postgresql://navigator-users:password@navigator-postgres:5432/users

# Auth Providers
ENV GITHUB_CLIENT_ID=github-client-id
ENV GITHUB_CLIENT_SECRET=github-client-secret

RUN bun tsoa

EXPOSE 8000

CMD ["sh", "-c", "bun drizzle:auth && bun run src/server.ts"]