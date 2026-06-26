# Compiler and flags
CC = dotnet

# Project name
PROJ = DotLox

# Output directories
BIN_DIR = bin
OBJ_DIR = obj

# Output binary
DBG = $(BIN_DIR)/Debug/net10.0/dotlox
REL = $(BIN_DIR)/Release/net10.0/dotlox

.PHONY: debug release script test-dbg test-rel clean

# Targets
debug: $(DBG)
release: $(REL)

# Link object files into final binary
$(DBG):
	python tool/generate_ast.py src/
	$(CC) build $(PROJ).csproj

$(REL):
	python tool/generate_ast.py src/
	$(CC) build $(PROJ).csproj --configuration Release

# Run code generation script
script:
	python tool/generate_ast.py src/

# Run tests
test-dbg:
	./$(DBG)
test-rel:
	./$(REL)

# Clean build artifacts via CLI parameter
clean:
	rm -rf $(OBJ_DIR) $(BIN_DIR)
