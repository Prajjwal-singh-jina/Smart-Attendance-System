from flask import Flask, request, jsonify
import face_recognition
import os
import cv2  # <--- WE ARE USING OPENCV NOW
import numpy as np

app = Flask(__name__)

# --- HELPER: LOAD WITH OPENCV (The Heavy Duty Way) ---
def load_image_safe(image_path):
    try:
        print(f"   [DEBUG] Loading: {image_path}")
        
        # 1. Load using OpenCV
        # OpenCV reads images as BGR (Blue-Green-Red) by default
        img = cv2.imread(image_path)
        
        if img is None:
            print("   [ERROR] OpenCV could not read the file. Check path/permissions.")
            return None

        # 2. Convert BGR to RGB
        # Dlib/Face_Recognition expects RGB. If we don't do this, the face will look blue!
        img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

        # 3. Debug Info
        print(f"   [DEBUG] Shape: {img_rgb.shape} | Type: {img_rgb.dtype}")
        
        return img_rgb

    except Exception as e:
        print(f"   [ERROR] Failed to load image: {e}")
        return None

@app.route('/verify', methods=['POST'])
def verify_face():
    try:
        data = request.json
        stored_path = data.get('stored_path')
        live_path = data.get('live_path')

        print("\n=== STARTING NEW SCAN (OPENCV V6) ===")

        if not os.path.exists(stored_path) or not os.path.exists(live_path):
            return jsonify({"match": False, "error": "File not found on server"})

        # Load Images
        img_stored = load_image_safe(stored_path)
        img_live = load_image_safe(live_path)

        if img_stored is None or img_live is None:
            return jsonify({"match": False, "error": "Could not read images (OpenCV failed)"})

        # Encode Stored
        try:
            enc_stored = face_recognition.face_encodings(img_stored)
        except Exception as e:
            print(f"   [CRITICAL] Stored Image Encoding Failed: {e}")
            return jsonify({"match": False, "error": "Profile Picture corrupted/invalid format"})

        if len(enc_stored) == 0:
            return jsonify({"match": False, "error": "No face in Profile Picture"})

        # Encode Live
        try:
            enc_live = face_recognition.face_encodings(img_live)
        except Exception as e:
            print(f"   [CRITICAL] Live Image Encoding Failed: {e}")
            return jsonify({"match": False, "error": "Camera Scan corrupted/invalid format"})

        if len(enc_live) == 0:
            return jsonify({"match": False, "error": "No face in Camera Feed"})

        # Compare
        match = face_recognition.compare_faces([enc_stored[0]], enc_live[0], tolerance=0.5)

        print(f"=== MATCH RESULT: {match[0]} ===\n")

        return jsonify({
            "match": bool(match[0]), 
            "error": None
        })

    except Exception as e:
        print(f"CRITICAL ERROR: {str(e)}")
        return jsonify({"match": False, "error": str(e)})

if __name__ == '__main__':
    print("AI Server V6 (OpenCV Engine) is running on Port 5000...")
    app.run(host='0.0.0.0', port=5000)