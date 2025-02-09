# Use the official Bun image as the base image
FROM oven/bun:latest

# Set the working directory inside the container
WORKDIR /app

# Copy dependency files first to leverage Docker layer caching
COPY package.json bun.lockb* ./

# Install dependencies
RUN bun install

# Copy the rest of your application code (all files and folders)
COPY . .
RUN bun tsoa

# Expose port 8000 (the port your app listens on)
EXPOSE 8000

# Start the application (adjust the entry point file if necessary)
CMD ["bun", "run", "server.ts"]
