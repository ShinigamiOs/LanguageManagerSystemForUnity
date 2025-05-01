
# LanguageManagerSystem

**LanguageManagerSystem** es un módulo reutilizable para Unity que permite gestionar múltiples idiomas dentro de un proyecto de manera eficiente y escalable. Incluye tanto la lógica de manejo de traducciones como herramientas visuales integradas en el editor de Unity para facilitar la creación, edición y mantenimiento de archivos de idiomas.

## Características

- ✅ Soporte para múltiples idiomas mediante archivos JSON.
- ✅ Definición de idioma principal y alternativos.
- ✅ Interfaz visual dentro del editor de Unity.
- ✅ Búsqueda, adición, edición y eliminación de claves de traducción.
- ✅ Separación clara entre código del sistema, editor y recursos.
- ✅ Preparado para integración en cualquier proyecto Unity (como paquete o módulo).

## Instalación

> Desarrollado en Unity 6 (6000.0.23)


```bash
git clone https://github.com/ShinigamiOs/LanguageManagerSystemForUnity.git
```

- Copiar la carpeta `LanguageSystem` dentro de la carpeta `Assets` de tu proyecto Unity.

## Uso básico

1. Abre Unity y accede a la ventana del Language Manager:
   - `Window > Language Manager System > Language Project Manager`
     <img src="[Extra/Tutorial-img/Captura de pantalla 01.png](https://github.com/ShinigamiOs/LanguageManagerSystemForUnity/blob/main/Extra/Tutorial-img/Captura%20de%20pantalla%2001.png)" alt="Captura de pantalla 01">
2. Crea o carga un proyecto de idioma.
    <img src="https://github.com/ShinigamiOs/LanguageManagerSystemForUnity/blob/main/Extra/Tutorial-img/Captura%20de%20pantalla%2002.png">
3. Agrega y edita claves de traducción desde el editor visual.
  <img src="https://github.com/ShinigamiOs/LanguageManagerSystemForUnity/blob/main/Extra/Tutorial-img/Captura%20de%20pantalla%2003.png">
4. Tambien puedes agregar tus claves modifcando direcctamente los archivos de lenguaje en la carpeta del proyecto de lenguaje cuando se genera
    en Assets/UnityLanguageManager/Resources/NombreDeProyectodeLenguaje/Languages/:
  <img src="https://github.com/ShinigamiOs/LanguageManagerSystemForUnity/blob/main/Extra/Tutorial-img/Captura%20de%20pantalla%2004.png">
  Por convencion se utilza la llave igual al valor en el texto definido como idioma principal pero no es obligatorio.
 Formato de los archivos por idioma.
  ```json
{
    "entries": [
        { "key": "Confirm", "value": "Confirmar" },
        { "key": "Cancel", "value": "Cancelar" },
        { "key": "Start", "value": "Iniciar" }
    ]
}
```
5. Una vez definidos los textos crea un GameObject al que se le agrega el LanguageManager y arrastra a este el archivo de proyecto que se creo.
   <img src="https://github.com/ShinigamiOs/LanguageManagerSystemForUnity/blob/main/Extra/Tutorial-img/Captura%20de%20pantalla%2005.png">
6. Puede hacerse referencia a este Singleton y utilizar sus distintas funciones.
```csharp
using LanguageSystem.Runtime;

String text = LanguageManager.Instance.LangString(key);

String textLower = LanguageManager.Instance.LangLower(key);

String textUpper = LanguageManager.Instance.LangUpper(key);

String textCapitalized = LanguageManager.Instance.LangCapitalized(key);

LanguageManager.Instance.SetLanguage(languageCode);
```
Se recomienda subscribir los usos de Lang al evento OnLanguageChanged, de esta manera con solo cambiar el lenguage en el singleton modifica todo los textos usados.
```csharp
private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }
    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= UpdateText;
    }
```
7. Otra manera de usarlo es por medio del LanguageStringDisplay, se agrega a un gameobject y se le define el TextMeshPro que cambiara el texto y la key que mostrara  
    Tambien puede agregarse directamente al TextMeshPro y este lo reconocera, este Script subscribe la actualizacion del texto a OnLanguageChanged por lo que 
    se actualiza automaticamente cuando el lenguaje cambia.
   <img src="https://github.com/ShinigamiOs/LanguageManagerSystemForUnity/blob/main/Extra/Tutorial-img/Captura%20de%20pantalla%2006.png">

## Contenido
```bash
📦 UnityLanguageSystemforUnity
├── Extra                       # Extras no necesarios para el proyecto
│   ├── Tutorial-img            # Capturas de pantalla para el tutorial
│   └── UnityLanguageManager    # Ejemplo de proyecto de lenguaje con palabras incluidas listo para usarse
│ 
│
└── LanguageSystem                  # Sistema 
    ├── Core                        # Necesarios para el funcionamiento del sistema
    ├── Demo                        # Escena Demo de su uso
    ├── Editor                      # Scripts necesario para el uso del sistema desde el editor(ventana, pop-up, etc)
    └── LanguageStringDisplay.cs    # Script para utilizar el sistema con TextMeshPro

```


## Licencia

> **GNU**

---

