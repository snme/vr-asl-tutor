import os
import pickle
import xgboost as xgb
import numpy as np
from flask import Flask, request, jsonify

app = Flask(__name__)

# internal state/model and scaler
with open('./scaler.pkl','rb') as f:
    scaler = pickle.load(f)

model = xgb.XGBClassifier()
model.load_model("alphabet.json")

full_names = ['ring_int_center',
 'pinkie_dist_dir',
 'ring_dir',
 'thumb_prox_center',
 'middle_int_center',
 'index_int_center',
 'middle_prox_dir',
 'middle_dist_center',
 'thumb_dir',
 'middle_tip_pos',
 'index_prox_dir',
 'pinkie_dir',
 'thumb_int_center',
 'ring_int_dir',
 'middle_int_dir',
 'ring_prox_center',
 'thumb_prox_dir',
 'index_meta_dir',
 'middle_prox_center',
 'pinkie_int_center',
 'ring_tip_pos',
 'pinkie_tip_pos',
 'middle_meta_center',
 'ring_prox_dir',
 'pinkie_meta_dir',
 'middle_dir',
 'index_dist_dir',
 'ring_meta_dir',
 'thumb_int_dir',
 'index_dir',
 'index_meta_center',
 'middle_meta_dir',
 'pinkie_meta_center',
 'pinkie_prox_center',
 'thumb_dist_center',
 'pinkie_prox_dir',
 'thumb_dist_dir',
 'pinkie_dist_center',
 'thumb_tip_pos',
 'thumb_meta_center',
 'thumb_meta_dir',
 'ring_dist_dir',
 'index_tip_pos',
 'index_prox_center',
 'ring_meta_center',
 'middle_dist_dir',
 'ring_dist_center',
 'index_int_dir',
 'hand_dir',
 'pinkie_int_dir',
 'index_dist_center']

@app.route('/classify', methods=['POST'])
def classify_hand_pos():
    if request.is_json:
        req = request.get_json()
        # single test example
        inp = req["data"]
        print(inp)
        # run model (scale and predict)
        """
        Rinv = np.linalg.inv(np.array([
            [inp['r11'], inp['r12'], inp['r13']],
            [inp['r21'], inp['r22'], inp['r23']],
            [inp['r31'], inp['r32'], inp['r33']],
        ]))
        t0 = np.array([inp['tx'], inp['ty'], inp['tz']])
        for vec in full_names:
            coord = np.array([inp[vec+'_1'], inp[vec+'_2'], inp[vec+'_3']])
            if 'center' in vec or 'pos' in vec:
                coord -= t0
                coord /= inp['s']
                coord = Rinv @ coord
            else:
                coord = Rinv @ coord
                coord /= np.linalg.norm(coord)
            inp[vec+'_1'], inp[vec+'_2'], inp[vec+'_3'] = coord
            """

        # data = scaler.transform(inp)
        inp = np.array(list(inp.values())[2:])
        data = inp.reshape(1, -1)
        guess = model.predict(data)
        # convert guess into letter and return
        print(guess)
        letter = chr(ord('a') + guess[0])
        return letter, 200
    return {"error": "Request must be JSON"}, 415
