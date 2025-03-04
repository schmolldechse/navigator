import { MongoClient } from "mongodb";

let cachedDb: MongoClient;

const connectToDb = async () => {
	if (cachedDb) return cachedDb.db("data");

	const client = await MongoClient.connect(process.env.MONGO_URL);
	cachedDb = client;
	return client.db("data");
};

export { connectToDb };