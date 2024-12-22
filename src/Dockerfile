FROM node:18-alpine

WORKDIR /app

COPY . .
RUN npm install
RUN npm run build
RUN npm start

CMD [ "npm", "run", "dev" ]

EXPOSE 3000
