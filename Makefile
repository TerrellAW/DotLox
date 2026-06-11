# Compiler and flags
CC = dotnet

# Project name
PROJ = DotLox

# Output directories
BIN_DIR = bin
OBJ_DIR = obj

# Output binary
BIN = $(BIN_DIR)/Debug/net10.0/DotLox

.PHONY: build test clean

# Default target
build: $(BIN)

# Link object files into final binary
$(BIN):
	python tool/generate_ast.py src/
	$(CC) build $(PROJ).csproj

# Run test
test:
	./$(BIN) test

# Clean build artifacts via CLI parameter
clean:
	rm -rf $(OBJ_DIR) $(BIN_DIR)
