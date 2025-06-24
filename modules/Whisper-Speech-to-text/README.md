# How to Use

## 1. Install Dependencies
1. Make sure you have `git` and `cmake` installed on your system.
    If you have an Nvidia GPU, you can also install nvidia-cuda-toolkit.
    This is optional, but it will speed up the inference.

## 2. Install Whisper.cpp
```bash
cd modules\Whisper-Speech-to-text
git clone https://github.com/ggml-org/whisper.cpp.git
cd whisper.cpp
cmake -B build -DGGML_CUDA=1 # If you have an Nvidia GPU. With a GPU in Series 50 you also need to add -DCMAKE_CUDA_ARCHITECTURES="86"
cmake --build build --config Release
```
