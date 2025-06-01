FROM oven/bun:latest
WORKDIR /app

COPY package.json bun.lock* ./

RUN bun install --production
COPY . .

RUN bun tsoa

EXPOSE 3000

CMD ["sh", "-c", "bun drizzle:generate && bun run drizzle:migrate && bun run src/server.ts"]