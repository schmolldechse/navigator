FROM redis:latest

WORKDIR /usr/local/etc/redis

COPY ./redis.conf /usr/local/etc/redis/redis.conf
COPY ./users.acl /usr/local/etc/redis/users.acl

RUN mkdir -p /data
RUN chmod -R 777 /data

CMD [ "redis-server", "/usr/local/etc/redis/redis.conf" ]
