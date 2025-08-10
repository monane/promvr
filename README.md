Используемые технологии:
 - Unity 6000.0.55f1
 - Meta XR Core SDK
 - Meta XR Interaction SDK
---
Ключевые скрипты расположены внутри директории [Assets/_Main/Scripts/](Assets/_Main/Scripts/)

Данные:
 - [DrawingSegment.cs](Assets/_Main/Scripts/Drawing/DrawingSegment.cs) - данные о сегменте рисунка (а конкретнее - настройки кисти и позиции точек на доске для рисования).
 - [DrawingBoardSnapshot.cs](Assets/_Main/Scripts/Drawing/DrawingBoardSnapshot.cs) - данные о состоянии **DrawingBoard** (а конкретнее - массив активных **DrawingSegment**'ов).
 - [BrushParams.cs](Assets/_Main/Scripts/Drawing/BrushParams.cs) - данные о настройке кисти (цвет, радиус).
 - [PointerDrawingSession.cs](Assets/_Main/Scripts/Drawing/PointerDrawingSession.cs) - вспомогательные данные [для отслеживания активной сессии рисования сегмента пальцем](Assets/_Main/Scripts/Drawing/DrawingBoardPokeBrushing.cs#L83C13-L92C14).

Логика:
 - [RenderTextureDrawing.cs](Assets/_Main/Scripts/Drawing/RenderTextureDrawing.cs) - отвечает за рисование на RenderTexture с помощью шейдеров.
 - [DrawingBoard.cs](Assets/_Main/Scripts/Drawing/DrawingBoard.cs) - "Доска для рисования", главный объект отвечающий за [конечную отрисовку DrawingSegment'ов с помощью RenderTextureDrawing](Assets/_Main/Scripts/Drawing/DrawingBoard.cs#L105).
 - [DrawingBoardPersistence.cs](Assets/_Main/Scripts/Drawing/DrawingBoardPersistence.cs) - отвечает за сохранение и загрузку состояния для **DrawingBoard**.
 - [DrawingBoardPokeBrushing.cs](Assets/_Main/Scripts/Drawing/DrawingBoardPokeBrushing.cs) - отвечает за отрисовку **DrawingSegment**'ов на **DrawingBoard** [в ответ на ввод пальцами](Assets/_Main/Scripts/Drawing/DrawingBoardPokeBrushing.cs#L78C9-L105C10).
 - [DrawingBoardControlPanel.cs](Assets/_Main/Scripts/Drawing/DrawingBoardControlPanel.cs) - "Панель управления для доски рисования", здесь все кнопки и связанные с ними эвенты.
 - [ProtectDrawingBoardWhenLoading.cs](Assets/_Main/Scripts/Drawing/ProtectDrawingBoardWhenLoading.cs) - вспомогательный класс, который защишает **DrawingBoard** от изменений во время активной загрузки данных в **DrawingBoardPersistence**.
 - [JsonStorage.cs](Assets/_Main/Scripts/Utils/JsonStorage.cs) - утилитарный класс, способствует простому асинхронному сохранению и загрузке [сериализуемых объектов](https://docs.unity3d.com/6000.0/Documentation/Manual/json-serialization.html).
---
Возможности расширения:
 - Можно легко через инспектор [добавить или изменить цвета](Assets/_Main/Scripts/Drawing/DrawingBoardPokeBrushing.cs#L18), не изменяя кода.
 - [В данных сегмента помимо цвета заложен и радиус кисти](Assets/_Main/Scripts/Drawing/DrawingSegment.cs#L9) - можно легко вывести на панель управления слайдер для настройки радиуса кисти.
 - За счёт использования Meta XR Interaction SDK можно с минимальными усилиями [прикрутить](Assets/_Main/Scripts/Drawing/DrawingBoardPokeBrushing.cs#L35) рисование с помощью другого метода ввода (например классический лазер из контроллера)

Производительность:
- Во время рисования в сцену не добавляются никакие новые объекты, потому что [логика рисования реализована через шейдер](Assets/_Main/Scripts/Drawing/RenderTextureDrawing.cs#L66C13-L67C67) - это значит, что вся тяжелая логика связанная с отрисовкой выполняется на стороне GPU. За счёт этого достигается хорошая производительность.
- Также, рисование новых точек осуществляется [только если палец сдвинулся от последней нарисованной точки на минимальный порог (threshold)](Assets/_Main/Scripts/Drawing/DrawingBoardPokeBrushing.cs#L96). Значение порога настраивается через инспектор. Это позволяет гарантировать, что во время держания пальца в одном месте, не будет происходить лишних отрисовок.
---
Ссылки:
- [Видео демонстрация](https://disk.yandex.ru/i/Tqvfj71aTl9BAg) - на записи не видно режима **pass-through**, но он включён.
- [APK-файл](https://disk.yandex.ru/d/kBvbclD3YlOE4Q)

Тестировалось на устройстве Oculus Quest 2 с версией ОС v78.0

Автор: Кирилл Патрин
