FROM node as restore
WORKDIR /app
COPY Web/Holefeeder.Web/package*.json .
RUN npm install

FROM restore as build
ENV NODE_OPTIONS="--max_old_space_size=8096"
WORKDIR /app
COPY Web/Holefeeder.Web/. .
RUN npm run build -- --prod

FROM nginx:alpine
RUN apk update && apk add jq

COPY --from=build /app/dist/ /usr/share/nginx/html
COPY --from=build /app/config/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
# generate dynamic json from env
COPY --from=build /app/docker-entrypoint.sh /
RUN chmod +x /docker-entrypoint.sh
ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]
