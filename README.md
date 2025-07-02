# Environment preparation

## 1. Install Git from its [WEB site](https://git-scm.com/downloads), choosing your operating system

## 2. Open a git/bash command prompt, or make sure that git executable is in the path

## 3. ***CD*** into the base folder for your git repositories
If you do not have one, you may create a folder called `git_repos`

## 4. Use `git` to clone this repo locally, then ***CD*** into that folder
```
git clone https://github.com/maurominella/aaas.git
cd aaas
```

## 5. Install Miniconda from its [WEB site](https://www.anaconda.com/docs/getting-started/miniconda/install), choosing your operating system

## 6. Open Miniconda bash / prompt, or make sure that conda executable is in the path

## 7. Environment provisioning for Azure AI Agent Service (`aaas`)

### 7.1 Remove the pre-existing conda `aaas` environment (if exists)
```conda env remove -n aaas -y```

### 7.2 Create new Conda Environment `aaas` with Python 3.13
```conda create -n aaas python=3.13 -y```

### 7.3 Activate the `aaas` environment
```conda activate aaas```

### 7.4 Install libraries and dependencies
```pip install -r requirements_aaas.txt```

### 7.5 Remove the kernel (if exists)
```jupyter kernelspec uninstall aaas -y```

### 7.6 Create the kernel 
```python -m ipykernel install --name azuremlsdkv2mm --user```

### 7.7 Check kernels list
```jupyter kernelspec list```

## 8. Create a sub-folder of the base `git_repos` called `config`
Don't move into this folder, just copy the file `credentials_my.env` into it:
`copy ./credentials_my.env ./config`
This file has to be updated with your own 
