import cv2
import numpy as np
import tflite_runtime.interpreter as tflite  # For tflite-runtime

# Function to calculate mask over image
def weighted_img(img, initial_img, α=1., β=0.5, γ=0.):
    return cv2.addWeighted(initial_img, α, img, β, γ)

# Function to process an individual image
def process_image(interpreter, image):
    # Preprocess image
    input_details = interpreter.get_input_details()
    input_shape = input_details[0]['shape'][1:3]  # Model expects height, width

    print(f"{input_shape=}\n")

    # Resize image to input shape
    resized_image = cv2.resize(image, (input_shape[1], input_shape[0]))
    
    print(f"{resized_image.shape=}\n")
    
    # Normalize image to [0, 1] if required by the model
    input_data = np.expand_dims(resized_image.astype(np.float32) / 255.0, axis=0)
    
    print(f"{input_data.shape=}\n")

    # Set the input tensor
    interpreter.set_tensor(input_details[0]['index'], input_data)

    print(f"{input_details[0]['index']=}\n")

    # Run the model
    interpreter.invoke()

    # Get the output (binary mask)
    output_details = interpreter.get_output_details()
    pred_mask = interpreter.get_tensor(output_details[0]['index'])

    print(f"{output_details=}\n")

    # Convert prediction to binary mask (assuming sigmoid output between 0 and 1)
    mask = (pred_mask[0] > 0.5).astype(np.uint8)

    # Convert to mask image (red color for segmentation mask)
    zero_image = np.zeros_like(mask)
    mask_colored = np.dstack((mask, zero_image, zero_image)) * 255  # Red mask

    # Resize mask back to original image size
    mask_resized = cv2.resize(mask_colored, (image.shape[1], image.shape[0]))

    # Blend mask with original image
    final_image = weighted_img(mask_resized, image)

    return final_image

# Load the TFLite model
model_path = "unity-env/Assets/StreamingAssets/Models/RoadSegmentation.tflite"
interpreter = tflite.Interpreter(model_path=model_path)

# Allocate tensors (prepare the model for inference)
interpreter.allocate_tensors()

# Example image (replace with actual image)
# image = cv2.imread("/home/things/Documents/KITTI/RoadSegmantation/data_road/testing/image_2/umm_000093.png")  # Load your image here
image = cv2.imread("RoadSegmantation/test4.png")  # Load your image here

# Process the image
final_output = process_image(interpreter, image)

# Save or display the final image
cv2.imshow("Final Segmentation", final_output)
cv2.waitKey(0)
cv2.destroyAllWindows()
