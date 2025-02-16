FROM node:alpine

WORKDIR /app

COPY . .
RUN npm install

ENV PUBLIC_BACKEND_BASE_URL=http://localhost:8000
ENV BACKEND_DOCKER_BASE_URL=http://navigator-backend:8000

RUN npm run build
RUN npm prune --production # Remove dev dependencies

EXPOSE 3000
ENV HOST=0.0.0.0

CMD ["node", "build/index.js"]