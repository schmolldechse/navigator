import {createClient, RedisClientType} from "redis";

const host = process.env.REDIS_HOST || "localhost";
const port = process.env.REDIS_PORT || 6379;
const user = process.env.REDIS_USER || "";
const password = process.env.REDIS_PASSWORD || "";

let client: RedisClientType | null = null;

export const getRedisClient = async (): Promise<RedisClientType> => {
    if (client) return client;

    const redisUrl = `redis://${user}:${password}@${host}:${port}`;

    client = createClient({ url: redisUrl });
    client.on('error', (err) => console.error("Redis Client error:", err));

    await client.connect();
    console.log("Redis Client connected");

    return client;
}
