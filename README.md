# American Sign Language in Virtual Reality: A Truly Interactive Fingerspelling Tutor
## Paper and Demo Video Links
Please read the paper below for information on this project!
[American Sign Language in Virtual Reality: A Truly Interactive Fingerspelling Tutor Paper](https://drive.google.com/file/d/1e00aHNuIpNkz2uvqOJvF6EL0_4jxz7lj/view?usp=sharing)

[Demo Video](https://drive.google.com/file/d/13ErgM_1xgqus3dsyA19Skj1vfJiY6FA5/view?usp=sharing)

## How to Run The Code
1. You'll need to install all of the Python 3.0 packages in the preamble of `classifier/model_api.py`. Set environment variables `FLASK_APP=model_api.py`, `FLASK_ENV=development`, and run `flask run` in your CLI in that directory after installation. This will start the classifier web server.
2. Install Unity (2018.4.20f1), Ultraleap for Unity (4.30), and Leap SDK (Orion v4).
3. Plug in Leap Motion (classic) device and make sure it's working as expected in the Visualizer.
4. Plug in Viewmaster as developed in EE 267. Mount the VRDuino on the Viewmaster and flash it with the `ee267_unitystarter/vrduino` files.
5. Lastly, run the executable in `ee267_unitystarter/Final\ Build/`. Please note that my classifier was

## Source Code Description
### Architecture:
<img width="400" alt="Screen Shot 2022-05-27 at 5 36 43 PM" src="https://user-images.githubusercontent.com/22281891/170802795-9448e43f-be98-4bf5-91a4-13d620303104.png">

This is the source code for my Virtual Reality American Sign Language Tutor (EE 267 Spring '22 Project). There's a few different folders that hold the various parts of my software architecture, shown above:
- `leap-data`: Scripts I used to connect the Leap motion device and my computer to construct my custom training dataset. `gather-data.html` is the main script for this—I would plug in my Leap motion device, form my hand into an ASL letter, and press the corresponding letter on my computer to log my hand position. After collecting all 2,653 individual hand positions in my dataset, I clicked "Create file" and "Download" to download my generated file. An old, poorly generated, smaller version of my dataset is available in `training-data-v1.csv'. The final dataset is located in `training-data.csv`.
- `classifier`: The folder that houses all of the training data exploration (`Exploratory Data Analysis.ipynb`) and model training (`Model Testing.ipynb`)  Python notebooks. `alphabet-simplified.json` is the final, pickled XGBoost classifier, and `alphabet.json` is a similar XGBoost classifier, except trained on more variables (check out the model training notebook for more information). `generate_unity_script.py` is a script I wrote to create the C# code in my main Unity code that obtained 153 individual measurements from the Leap SDK (I didn't want to write it by hand). **Importantly,** `model_api.py` is the Flask REST API endpoint that I built around the classifier. 
- `ee267_unitystarter`: The unity project that houses my game. The executable in `Final\ Build` is my final product—just a warning that my dataset only consists of my hand, so your performance may be limited. Besides the contents of the `Assets` folder, I'd like to emphasize `ee267_unitystarter/Assets/EventSystem.cs`, as it's the script that controls the entire game, invokes the classifier, and implements the FSM for our game.
- `hand-tracking-demo`: The unity project I started developing the Leap motion tracker in. Useful as a demonstration of the right packages and version of Unity (2018.4.20f1), Ultraleap for Unity (4.30), and Leap SDK (Orion v4) in case anyone would like to adopt a similar strategy for their project.
