FROM node:0.10-slim

WORKDIR /app
ADD package.json /app/
RUN npm install
ADD server.js /app/

EXPOSE 1337
ENTRYPOINT ["node", "server.js"]
