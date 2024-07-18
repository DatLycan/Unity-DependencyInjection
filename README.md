
<h1 align="left">Unity C# Dependency Injection</h1>

<div align="left">

[![Status](https://img.shields.io/badge/status-active-success.svg)]()
[![GitHub Issues](https://img.shields.io/github/issues/datlycan/Unity-DependencyInjection.svg)](https://github.com/DatLycan/Unity-DependencyInjection/issues)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](/LICENSE)

</div>

---

<p align="left"> Custom Dependency Injection for Unity that allows providing and injection of dependencies between different scripts without direct references.
    <br> 
</p>

## 📝 Table of Contents

- [Getting Started](#getting_started)
- [Usage](#usage)
- [Acknowledgments](#acknowledgement)

## 🏁 Getting Started <a name = "getting_started"></a>

### Installing

Simply import it in Unity with the Unity Package Manager using this URL:

``https://github.com/DatLycan/Unity-DependencyInjection.git``

## 🎈 Usage <a name="usage"></a>


   ```C#
    public class MyClassA : MonoBehaviour, IDependencyProvider {
        [Provide] private string ProvideString() => "My Provided String";
    }
   ```
   ```C#
    public class MyClassB : MonoBehaviour {
        [Inject] private string myString; // Output {My Provided String}
        // OR
        [inject] private void InjectIntoMyString(string injectedString) => myOtherString = injectedString;
        private string myOtherString // Output {My Provided String}
    }
   ```
---



## 🎉 Acknowledgements <a name = "acknowledgement"></a>

- *Inspired by from [adammyhre's Dependency Injection video](https://www.youtube.com/watch?v=PJcBJ60C970).*

