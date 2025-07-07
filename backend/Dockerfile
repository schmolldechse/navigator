FROM oven/bun:latest
WORKDIR /app

COPY package.json bun.lock* ./

RUN bun install --production
COPY . .

EXPOSE 3000

CMD ["sh", "-c", "bun run drizzle:migrate && bun run src/server.ts"]