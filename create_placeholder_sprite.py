from PIL import Image, ImageDraw, ImageFont
import os

# Create directory if it doesn't exist
output_dir = r"c:\VelociFamily\KnowEyeDia\Assets\Art\Sprites\Characters"
os.makedirs(output_dir, exist_ok=True)
output_path = os.path.join(output_dir, "Astronaut_Placeholder.png")

# Grid settings
params = {
    "frame_width": 32,
    "frame_height": 32,
    "cols": 4, # 4 frames per animation
    "rows": 9  # 1 Idle + 8 Directions
}

width = params["cols"] * params["frame_width"]
height = params["rows"] * params["frame_height"]

img = Image.new('RGBA', (width, height), (0, 0, 0, 0))
draw = ImageDraw.Draw(img)

# Row definitions
row_labels = [
    "Idle",
    "N", "NE", "E", "SE", 
    "S", "SW", "W", "NW"
]

# Colors for visual distinction
colors = [
    (200, 200, 255, 255), # Idle - Light Blue
    (255, 200, 200, 255), # N - Light Red
    (255, 225, 200, 255), # NE
    (255, 255, 200, 255), # E - Light Yellow
    (225, 255, 200, 255), # SE
    (200, 255, 200, 255), # S - Light Green
    (200, 255, 225, 255), # SW
    (200, 255, 255, 255), # W - Light Cyan
    (200, 225, 255, 255)  # NW
]

try:
    # Attempt to load a font, fall back to default if needed
    font = ImageFont.load_default()
except:
    font = None

for row in range(params["rows"]):
    label = row_labels[row]
    color = colors[row % len(colors)]
    
    for col in range(params["cols"]):
        x = col * params["frame_width"]
        y = row * params["frame_height"]
        
        # Draw background rect
        draw.rectangle([x, y, x + params["frame_width"] - 1, y + params["frame_height"] - 1], fill=color, outline=(0,0,0,255))
        
        # Draw text: "Dir\nFrame"
        text = f"{label}\n{col}"
        
        # Simple centering approximation
        if font:
            draw.text((x + 2, y + 2), text, font=font, fill=(0, 0, 0, 255))
        else:
            draw.text((x + 2, y + 2), text, fill=(0, 0, 0, 255))

img.save(output_path)
print(f"Created placeholder sprite sheet at: {output_path}")
