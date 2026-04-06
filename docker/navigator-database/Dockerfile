FROM postgres:18.3

RUN apt update && \
    apt install -y postgresql-18-cron postgresql-18-partman && \
    rm -rf /var/lib/apt/lists/*