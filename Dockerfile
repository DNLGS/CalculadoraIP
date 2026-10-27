FROM nginx:alpine

COPY bin/Release/net10.0/publish/wwwroot/ /usr/share/nginx/html/

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80