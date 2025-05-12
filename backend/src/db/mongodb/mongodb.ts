import { Collection, Db, MongoClient } from "mongodb";
import type { ConnectionDocument, StationDocument } from "./data.schema.ts";

interface Collections {
	stations: StationDocument;
	trips: ConnectionDocument;
}

let cachedDb: Db | null = null;

const connectToDb = async (): Promise<Db> => {
	if (!Bun.env.MONGO_URL) throw new Error("Could not establish a connection to the database");

	cachedDb ??= (await MongoClient.connect(Bun.env.MONGO_URL)).db("data");
	return cachedDb;
};

const getCollection = async <T extends keyof Collections>(name: T): Promise<Collection<Collections[T]>> => {
	const db = await connectToDb();
	const collection = db.collection<Collections[T]>(name);

	if (!collection) throw new Error(`Failed to resolve collection: ${name}`);
	return collection;
};

export { type StationDocument, connectToDb, getCollection };
