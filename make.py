#!/usr/bin/python
import os, shutil, sys

def colored(r, g, b, text):
    return f"\033[38;2;{r};{g};{b}m{text} \033[38;2;255;255;255m"

status = os.system("dotnet build")
if status != 0:
    print("Build failed, exiting")
    exit(1)
for file in os.listdir():
    if os.path.isfile(os.path.abspath(file)): continue
    if not file.lower().endswith("extension"): continue
    paths = [os.path.abspath(f"./{file}/bin/Debug/net6.0/{file}.dll"), f"{os.environ['HOME']}/.cruncheditor/Extensions/{file}"]
    if os.path.exists(paths[0]):
        if not os.path.exists(paths[1]): os.mkdir(paths[1])
        shutil.copyfile(paths[0], f"{paths[1]}/Extension.so")
        print(colored(104, 224, 13, f"{paths[0]} -> {paths[1]}/Extension.so"))
if len(sys.argv) > 1 and sys.argv[1] == "run":
    os.chdir("CrunchCore")
    os.system("dotnet run")
    os.chdir("..")