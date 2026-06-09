import sys
import io


# Source - https://stackoverflow.com/a/14981125
# Posted by MarcH, modified by community. See post 'Timeline' for change history
# Retrieved 2026-06-09, License - CC BY-SA 4.0
def eprint(*args, **kwargs):
    print(*args, file=sys.stderr, **kwargs)


# Handle CLI args
if (len(sys.argv) != 2):
    eprint("Usage: python generate_ast.py <output directory>")
    exit(64)

output_dir = sys.argv[1]

# AST metaprogramming
ast_types = [ 
    "Binary    : Expr left, Token opr, Expr right",
    "Grouping  : Expr expressions",
    "Literal   : Object value",
    "Unary     : Token opr, Expr right"
]

def define_type(f, class_name, fields_str):
    f.write("\tclass %s {\n" % class_name)
    f.write("\t\tpublic %s(%s) {\n" % (class_name, fields_str))
    
    # Write constructor
    fields = fields_str.split(", ")
    for field in fields:
        name = field.split(" ")[1]
        f.write("\t\t\tthis.%s = %s;\n" % (name, name))

    f.write("\t\t}\n\n")

    # Write fields
    for field in fields:
        f.write("\t\treadonly %s;\n" % field)

    f.write("\t}\n")

def define_ast(output_dir, base_name, types = [], *args):
    path = f"{output_dir}/{base_name}.cs"
    
    # Write abstract class and subclasses
    f = open(path, 'w')
    f.write("namespace DotLox;\n")
    f.write("\n")
    f.write("public abstract class %s {\n" % base_name)

    for type in types:
        class_name  = type.split(":")[0].strip()
        fields      = type.split(":")[1].strip()

        define_type(f, class_name, fields)

    f.write("}")
    f.close()

define_ast(output_dir, "Expr", ast_types)
