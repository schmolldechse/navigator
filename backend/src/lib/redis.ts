import { createClient, type RedisClientType } from "redis";

const host: string = process.env.REDIS_HOST || "localhost";
const port: number = Number.parseInt(process.env.REDIS_PORT ?? "") || 6379;
const user: string = process.env.REDIS_USER || "";
const password: string = process.env.REDIS_PASSWORD || "";
const database: number = Number.parseInt(process.env.REDIS_DATABASE ?? "") || 0;

let client: RedisClientType | null = null;

export const getRedisClient = async (): Promise<RedisClientType> => {
	if (client) return client;

	const redisUrl = `redis://${user}:${password}@${host}:${port}/${database}`;

	client = createClient({ url: redisUrl });
	client.on("error", (err) => console.error("Redis client error: ", err));

	await client.connect();
	console.log("Redis client connected");

	return client;
};
