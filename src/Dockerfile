FROM node:18-alpine

WORKDIR /app

COPY . .
RUN npm install
RUN npm run build
RUN npm start

EXPOSE 3000
