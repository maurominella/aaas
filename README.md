# Environments preparation

## 1. Install Miniconda from its [WEB site](https://www.anaconda.com/docs/getting-started/miniconda/install), choosing your operating system

## 2. Open Miniconda bash / prompt, or make sure that conda executable is in the path

## 3. First environment provisioning for Azure AI Agent Service (`aaas`)

### 3.1 Remove the pre-existing conda environment (just if you have a previous one, however running this does not hurt ;-))
```conda env remove -n aaas -y```

### 3.2 Prepare & Activate Conda Environment with Python 3.13
```conda create -n aaas python=3.13 -y```

### 3.2 Activate the environment
```conda activatye aaas```

### 1.3 Install libraries and dependencies
```pip install -r requirements_aaas.txt```

### 1.4 Remove the kernel (just if exists, however running this does not hurt ;-))
```jupyter kernelspec uninstall aaas -y```

### 1.3 Install libraries and dependencies
```pip install -r requirements_aaas.txt```

### Prepare & Activate Conda Environment and install basic libraries:

Libraries:  pip install pip install openai python-dotenv azure-identity jupyter langchain langchain-community langchain-openai numexpr pandas
Create kernel: python -m ipykernel install --user --name=openai
![image](https://github.com/user-attachments/assets/9a928114-55e6-4dd6-bfcc-5a716352db1c)

