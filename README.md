
# Autonomous Car in Unity

The project mainly aims to develop an autonomous car using a Unity 3D environment and complete the [NHTSA Levels of Automation](https://www.nhtsa.gov/sites/nhtsa.gov/files/2022-05/Level-of-Automation-052522-tag.pdf) one by one. See [Roadmap](#roadmap) and [Technologies](#technologies) for more detail.

## Roadmap

- [x] Simple Unity 3D Environment
  - The environment should be suitable for object detection tasks.
  - Car controls must be realistic enough to simulate an autonomous vehicle.
  - Integration with stereo camera sensors for realistic input data.

- [ ] Road Segmantation and Lane Detection

- [ ] Model Training with Reinforcement Learning for [Level 1](https://www.nhtsa.gov/sites/nhtsa.gov/files/2022-05/Level-of-Automation-052522-tag.pdf) Vehicle (Steering) Assist.

- [ ] 3D Object Detection Model (KITTI Dataset)
  - Train a 3D object detection model using the KITTI dataset.
  - The autonomous car should rely on stereo cameras (no LiDAR).
  - Apply the trained model to detect and localize objects in the Unity environment.

- [ ] Bird’s Eye View (BEV) Transformer System
  - Implement a BEV Transformer System for 3D space prediction.
  - Utilize stereo vision to predict the 3D structure of the environment.
  - Explore combining the stereo vision system with more advanced models for enhanced accuracy.

- [ ] [Level 2](https://www.nhtsa.gov/sites/nhtsa.gov/files/2022-05/Level-of-Automation-052522-tag.pdf) Vehicle Assist

- [ ] Rule-based System Design for Realistic Traffic Ride

- [ ] [Level 3](https://www.nhtsa.gov/sites/nhtsa.gov/files/2022-05/Level-of-Automation-052522-tag.pdf) Vehicle Assist

- [ ] [Level 4](https://www.nhtsa.gov/sites/nhtsa.gov/files/2022-05/Level-of-Automation-052522-tag.pdf) Vehicle (Parking in Selected Parking Areas and Highway Riding) Assist

- [ ] [Level 5](https://www.nhtsa.gov/sites/nhtsa.gov/files/2022-05/Level-of-Automation-052522-tag.pdf) Vehicle (Currently Dreaming) Assist

## Documentation

See the detailed [`documentation`](documentaion.html).

## Notes

See my [`notes`](https://elymsyr0000.notion.site/autonomous-systems-python-13652999cef8802a943ae7a23eb94d25?pvs=4).

## License

See the [`LICENSE`](LICENSE)

## Screenshots

Screenshots will be updated.

## Technologies

Technologies will be updated.

## Installation

Installation will be updated.

## Run Locally

Local Run will be updated.

## Support

For support, email [orhun868@gmail.com](mailto:orhun868@gmail.com).

## Contributing

All contributions are welcome. 
See [`CONTRIBUTING`](CONTRIBUTING.md) for ways to get started.
Please adhere to this project's [`CODE OF CONDUCT`](CODEOFCONDUCT.md).

## Acknowledgements

- [Datasets](https://www.cvlibs.net/datasets/kitti/eval_object.php?obj_benchmark=3d)
- [CNN for Road Segmantation](https://www.kaggle.com/code/sakshaymahna/fully-convolutional-network)

## Appendix

Appendix will be updated.
