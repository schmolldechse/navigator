FROM node:18-alpine

WORKDIR /usr/src/hafas-frontend

COPY . .
RUN npm install

CMD [ "npm", "run", "dev" ]

EXPOSE 3000
