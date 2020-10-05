import os

metaFiles = [f for f in os.listdir(".") if ".meta" in f]

for file in metaFiles:
	with open(file, "rb") as f:
		buf = f.read()
	buf = buf.replace("spritePixelsToUnits: 100",  "  spritePixelsToUnits: 27",)
	with open(file, "wb") as f:
		f.write(buf)