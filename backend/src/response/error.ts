class HttpError extends Error {

	public readonly httpStatus: number;
	public readonly message: string;

	constructor(httpStatus: number, message: string) {
		super(message);

		this.httpStatus = httpStatus;
		this.message = message;

		Object.setPrototypeOf(this, HttpError.prototype);
		Error.captureStackTrace(this)
	}
}

export { HttpError };