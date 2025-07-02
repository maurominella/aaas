# Environments preparation

## 1. Install Miniconda from its [WEB site](https://www.anaconda.com/docs/getting-started/miniconda/install), choosing your operating system

## 2. Open Miniconda bash / prompt, or make sure that conda executable is in the path

## 3. Environment provisioning for Azure AI Agent Service (`aaas`)

### 3.1 Remove the pre-existing conda `aaas` environment (if exists)
```conda env remove -n aaas -y```

### 3.2 Create new Conda Environment `aaas` with Python 3.13
```conda create -n aaas python=3.13 -y```

### 3.3 Activate the `aaas` environment
```conda activate aaas```

### 3.4 Install libraries and dependencies
```pip install -r requirements_aaas.txt```

### 3.5 Remove the kernel (if exists)
```jupyter kernelspec uninstall aaas -y```

### 3.6 Create the kernel 
```python -m ipykernel install --name azuremlsdkv2mm --user```

### 3.7 Check kernels list
```jupyter kernelspec list```
