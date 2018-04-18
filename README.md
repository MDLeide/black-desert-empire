# Black Desert Online: Empire
_A utility written for Black Desert Online to support economic activities._

_[The application is no longer being updated. In this, its final state, it remains unoptimized. Most of the features should work, however, the data loading strategy was hastily and naievely implemented, so load times can be a bit annoying. There may also be application crashing bugs in certain cases.]_
## Summary
BDO: Empire is a software utility designed to help a player efficiently grow their wealth in the massive multiplayer online role playing game Black Desert Online – a game with complex economic systems and a regulated marketplace. By capturing and analyzing data about the state of the in-game marketplace, Empire is able to identify economic opportunities and provide intelligence on how to exploit them. These features are conveniently exposed in a configurable UI which has been designed to feel at home on top of the main game window.

It is written primarily in C#, on Microsoft’s .NET platform, with Windows Presentation Foundation (WPF) providing the backbone for the UI, which is implemented using the MVVM pattern. In-game market data is acquired by analyzing image data from the game client, requested through the Win32 API, using the Teserect OCR engine. The data is housed in a SQLite DB, managed by nHibernate.

## Details


BDO: Empire is a software utility designed to help a player efficiently grow their wealth in the massive multiplayer online role playing game Black Desert Online – a game with complex economic systems and a regulated marketplace. By capturing and analyzing data about the state of the in-game marketplace, Empire is able to identify economic opportunities and provide intelligence on how to exploit them. These features are conveniently exposed in a configurable UI which has been designed to feel at home on top of the main game window.

The primary method of income supported by Empire is the sale of finished goods crafted from raw materials purchased from the market. Others were considered and tested, such as arbitrage and aggressive market manipulation, but ultimately were discarded. By evaluating each crafting recipe for profitability, considering market states and crafting skill, Empire identifies recipes that offer the greatest return. Those recipes can then be tracked, and each one’s crafting tree explored.

Market data is modeled as a market observation at the item level. These can be entered manually, or captured in a semi-automated fashion by performing OCR on the video data while viewing the marketplace in the BDO client. Several other observations are required to approach an accurate model, such as craft yield and energy consumption, which are manually input.

Each crafting recipe is quantitatively ranked as a function of cost, revenue and volume. Material availability is also considered. Cost and revenue can be evaluated several ways, according to configuration. A suitable recipe has materials which can be reliably purchased from the market – or which can be crafted from materials reliably available – and whose finished goods sell at a sufficient profit and volume. 

A word on the underlying dataset – there was no available resource, which I could find, providing structured data representing the game domain (i.e., the recipes, items, and their properties). The initial set of data was scraped from a website by parsing its HTTP responses using XPath. This was an efficient and effective method, but irregularities have arisen at times and caused pain. There also exist facilities in the GUI to operate on the records.

The GUI, driven by WPF and implemented along the MVVM pattern, was designed with two driving tenets: easy, efficient access to information during gameplay; minimal impact when used in a single-monitor full-screen environment. The first was honored with a careful workflow design and preference for graphics with a high information density. The rich graphics also supported the second tenet, which was further upheld by the minimalist theme and style.
All of this comes together to form a utility which provides access to item and market data, as well as the results of evaluations on that data, enabling a player to make smart choices about spending their time and silver. 
