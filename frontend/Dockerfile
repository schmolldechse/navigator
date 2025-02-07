FROM node:alpine

WORKDIR /app

COPY . .
RUN npm install
RUN npm run build
RUN npm prune --production # Remove dev dependencies

EXPOSE 3000
ENV HOST=0.0.0.0

CMD ["node", "build/index.js"]