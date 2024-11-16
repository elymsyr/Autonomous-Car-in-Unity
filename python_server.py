import socket
import threading
import numpy as np
import cv2
import struct, select

def process_image(image_bytes):
    image = cv2.imdecode(np.frombuffer(image_bytes, np.uint8), cv2.IMREAD_COLOR)
    if image is None:
        # print("None found")
        return None
    image = cv2.resize(image, (640, 480))
    return image

def handle_client_connection(client_socket, client_address, camera_id):
    print(f"Connection established with camera {camera_id} at {client_address}")
    cv2.namedWindow(f"Camera {camera_id}", cv2.WINDOW_NORMAL)

    try:
        while True:
            ready_to_read, _, _ = select.select([client_socket], [], [], 1)
            if ready_to_read:
                length_prefix = b""
                while len(length_prefix) < 4:
                    # print('LenLoop')
                    packet = client_socket.recv(4 - len(length_prefix))
                    if not packet:
                        print(f"Camera {camera_id} connection closed unexpectedly.")
                        return
                    length_prefix += packet

                data_length = struct.unpack('<I', length_prefix)[0]

                # print(data_length)

                image_data = b""
                while len(image_data) < data_length:
                    # # print('ImageLoop')
                    packet = client_socket.recv(min(4096, data_length - len(image_data)))
                    if not packet:
                        print(f"Camera {camera_id} connection closed while receiving image data.")
                        return
                    image_data += packet

                image = process_image(image_data)

                if image is not None:
                    cv2.imshow(f"Camera {camera_id}", image)

                    if cv2.waitKey(1) & 0xFF == ord('q'):
                        break
                else:
                    print(f"Failed to decode image from camera {camera_id}")
                
    except Exception as e:
        print(f"Error with camera {camera_id}: {e}")
    finally:
        client_socket.close()
        cv2.destroyWindow(f"Camera {camera_id}")

def start_server():
    camera_ports = [3010, 3020, 3030]
    camera_threads = []

    for camera_id, port in enumerate(camera_ports):
        thread = threading.Thread(target=start_camera_server, args=(camera_id, port))
        camera_threads.append(thread)
        thread.start()

    for thread in camera_threads:
        thread.join()

def start_camera_server(camera_id, port):
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server.bind(('127.0.0.1', port))
    server.listen(5)
    print(f"Listening on port {port} for camera {camera_id}...")

    while True:
        client_socket, client_address = server.accept()
        client_handler = threading.Thread(target=handle_client_connection, args=(client_socket, client_address, camera_id))
        client_handler.start()

if __name__ == "__main__":
    start_server()
