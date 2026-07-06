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

.PHONY: debug release script install test clean

# Targets
debug: $(DBG)
release: $(REL)

# Build with dotnet
$(DBG):
	$(CC) build $(PROJ).csproj
$(REL):
	$(CC) build $(PROJ).csproj --configuration Release

# Run code generation script
script:
	python tool/generate_ast.py src/

# Install to /usr/bin
install:
	@if [ -f $(REL) ]; then			\
		sudo mv $(REL)* /usr/bin; 	\
	elif [ -f $(DBG) ]; then 		\
		sudo mv $(DBG)* /usr/bin; 	\
	fi

# Run tests
test:
	@if [ -f $(REL) ]; then		\
		./$(REL);				\
	elif [ -f $(DBG) ]; then	\
		./$(DBG);				\
	fi

# Clean build artifacts via CLI parameter
clean:
	rm -rf $(OBJ_DIR) $(BIN_DIR)
