FROM oven/bun:latest AS install_stage

WORKDIR /app

COPY package.json bun.lock* ./

RUN bun install


FROM install_stage AS migration_stage

WORKDIR /app

RUN bun run drizzle:migrate


FROM install_stage AS production_stage

WORKDIR /app

COPY . .

EXPOSE 3000

CMD ["bun", "run", "src/server.ts"]