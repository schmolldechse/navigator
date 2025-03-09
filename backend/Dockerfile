FROM oven/bun:latest
WORKDIR /app

COPY package.json bun.lockb* ./

RUN bun install
COPY . .

# MongoDB
ENV MONGO_URL=mongodb://lo-u-data:password@navigator-mongo:27017/data

# BetterAuth
ENV AUTH_SECRET=auth-secret
ENV AUTH_POSTGRES_URL=postgresql://navigator-users:password@navigator-postgres:5432/users

# Auth Providers
ENV GITHUB_CLIENT_ID=github-client-id
ENV GITHUB_CLIENT_SECRET=github-client-secret

RUN bun tsoa

EXPOSE 8000

CMD ["sh", "-c", "bun drizzle:generate && bun run drizzle:migrate && bun run src/server.ts"]