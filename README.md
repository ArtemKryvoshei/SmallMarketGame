# Структура проекта Unity / Unity Project Structure

## Главные папки / Main Folders

- **Core** — содержит универсальные модули, не привязанные к конкретной игре.  
  **Core** — contains universal modules not directly related to game content.

- **Content** — основная рабочая директория, включает в себя все игровые модули.  
  **Content** — main working directory, contains all gameplay-related modules.

---

## Структура папки Content / Content Folder Structure

Внутри папки `Content` находятся две ключевые подпапки:  
Inside the `Content` folder, there are two main subfolders:

- **Global** — глобальные файлы, не относящиеся к конкретным фичам.  
  **Global** — global files that are not part of specific features.

- **Features** — модули отдельных игровых механик и фич.  
  **Features** — individual gameplay feature modules.

---

## Структура модуля / Feature Module Structure

Каждый модуль может содержать до трёх папок (некоторые из них могут отсутствовать, если не требуются):

Each module may contain up to three folders (some may be missing if not needed):

- **Scripts** — скрипты и логика поведения.  
  **Scripts** — scripts and behavioral logic.

- **Sources** — исходные медиа-ресурсы (спрайты, звуки, анимации и т.д.).  
  **Sources** — raw media assets (sprites, sounds, animations, etc.).

- **GameResources** — элементы, напрямую используемые в игре: префабы, ScriptableObject'ы, сцены и пр.  
  **GameResources** — game-ready assets such as prefabs, ScriptableObjects, scenes, etc.

