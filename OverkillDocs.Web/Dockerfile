FROM node:22-alpine3.21 as builder
WORKDIR /app
COPY . .
RUN npm install
RUN npx ng build

FROM nginx:1.29.1-alpine
RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=builder /app/dist/OverkillDocs.Web /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
