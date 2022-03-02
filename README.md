# Documentación Práctica 1
___________________________________________________________________________


# Video de pruebas [aquí](https://youtu.be/ztK_XIu5nJk)

## Autores

##### Grupo 02
López Benítez, Ángel   -   angelo06@ucm.es
Rave Robayo, Jose Daniel   -   jrave@ucm.es
Prado Echegaray, Iván   -   ivprado@ucm.es
Mendoza Reyes, Juan Diego   -   juandiem@ucm.es


## Resumen

La práctica consiste en implementar un prototipo de una simulación del mito del minotauro y perseo, consistente en la aventura del héroe buscando la salida del laberinto, mientras el minotauro, una criatura mitológica mitad toro mitad persona, le busca 
para acabar con su vida, perseo debe encontrar la salida siguiendo el hilo de Ariadna, que ignora la magia del laberinto.
La práctica consta de un laberinto divido por caillas en el que se encontraran: perseo(el jugador) y el minotauro.
El laberinto se genera mediante la lectura de un documento de texto con un tamaño configurable y con más de una salida, las salidas tendrán distinta longitud desde el punnto de salida de perseo y tendrán distinta anchura.
El minotauro empieza en el centro del laberinto y merodea por los caminos del mismo hasta que ve a perseo, en este momento empieza a perseguirlo.
Perseo es nuestro personaje controlable, el cual se mueve con las flechas del teclado, si presionamos espacio, este control se detiene y perseo empieza a seguir el hilo de Ariadna por su cuenta; Perseo no puede camina a través de la casilla donde está el
minotauro.
El hilo de Ariadna se muestra cuando pulsamos el espacio, dibujándose una línea blanca por el camino que sigue perseo, y resaltando las casillas en blanco, el hilo va por el camino más eficiente.

## Descripción Punto de Partida

# [Commit](https://github.com/IAV22-G02/P2/commit/ed26cda0429e6a28e262607879a93b947d5fc54e) de Punto de Partida 

La escena incial contiene un grid con un mapa de prueba, modelos para el minotauro y teseo, materiales y prefabs para los obstaculos además de los siguientes scripts:

BinaryHip: Una pila de información útil para ordenar datos e implementar colas de prioridad.
Edge: Conexión entre nodos, útil para calcular el coste de atravesar una casilla.
Graph: Clase abstracta para implementar grafos.
GraphGrid: Clase que genera el mapa a partir de un archivo .map
TesterGrahh: Clase que contiene distintos algoritmos para encontrar caminos en grafos.
Vertex: Cada uno de los vértices del grafo

## Descripción de la solución

La solución consta de la implementación de 3 nuevos componentes:
+ Componente MapGenerator, que se encarga de la generación prodecdural de laberintos.
+ El componente FindPath, que usaremos para encontrar el camino más corto mediante el hilo de Ariadna.
+ El componente FollowPath, que llevará a Teseo a seguir el camino que le indique el hilo de Ariadna.

### Opcionales

La solución también consta de funcionalidades opcionlaes tales como:
+ Generación procedimental de laberintos

El pseudocódigo de dichos componentes:

## MazeGenerator (Map)
```python
function maze(level: Level, start: Location):
 # A stack of locations we can branch from. locations = [start]
 level.startAt(start)

 while locations:
 current = locations.top()

 # Try to connect to a neighboring location.
 next = level.makeConnection(current)
 if next:
 # If successful, it will be our next iteration.
 locations.push(next)
 else:
 locations.pop()

class Level:
	function startAt(location: Location)
	function makeConnection(location: Location) -> Location

class Location:
	x: int
	x: int
	x: int
	y: int

 class Connections:
	inMaze: bool = false
	directions: bool[4] = [false, false, false, false]

 class GridLevel:
	# dx, dy, and index into the Connections.directions array.
	NEIGHBORS = [(1, 0, 0), (0, 1, 1), (0, -1, 2), (-1, 0, 3)]

	width: int
	height: int
	cells: Connections[width][height]

	function startAt(location: Location):
	cells[location.x][location.y].inMaze = true

 function canPlaceCorridor(x: int, y: int, dirn :int) -> bool:
	# Must be in-bounds and not already part of the maze.
	return 0 <= x < width and
	0 <= y < height and
	not cells[x][y].inMaze

 function makeConnection(location: Location) -> Location:
	# Consider neighbors in a random order.
	neighbors = shuffle(NEIGHBORS)

	x = location.x
	y = location.y
	for (dx, dy, dirn) in neighbors:

		# Check if that location is valid.
		nx = x + dx
		ny = y + dy
		fromDirn = 3 - dirn
		if canPlaceCorridor(nx, ny, fromDirn):

		#Perform the connection.
		cells[x][y].directions[dirn] = true
		cells[nx][ny].inMaze = true
		cells[nx][ny].directions[fromDirn] = true return Location(nx, ny)

 # null of the neighbors were valid.
 return null
```

## PathFinder(Teseo)
```python

 function pathfindAStar(graph: Graph,
 start: Node,
 end: Node,
 heuristic: Heuristic
 ) -> Connection[]:
 # This structure is used to keep track of the
 # information we need for each node.
 class NodeRecord:
 node: Node
 connection: Connection
 costSoFar: float
 estimatedTotalCost: float
 
 # Initialize the record for the start node.
 startRecord = new NodeRecord()
 startRecord.node = start
 startRecord.connection = null
 startRecord.costSoFar = 0
 startRecord.estimatedTotalCost = heuristic.estimate(start)
 
 # Initialize the open and closed lists.
 open = new PathfindingList()
 
 open += startRecord
 closed = new PathfindingList()
 
 # Iterate through processing each node.
 while length(open) > 0:
 # Find the smallest element in the open list (using the
 # estimatedTotalCost).
 current = open.smallestElement()
 
 # If it is the goal node, then terminate.
 if current.node == goal:
 break
 
 # Otherwise get its outgoing connections.
 connections = graph.getConnections(current)
 
 # Loop through each connection in turn.
 for connection in connections:
 # Get the cost estimate for the end node.
 endNode = connection.getToNode()
 endNodeCost = current.costSoFar + connection.getCost()
 
 # If the node is closed we may have to skip, or remove it
 # from the closed list.
 if closed.contains(endNode):
 # Here we find the record in the closed list
 # corresponding to the endNode.
 endNodeRecord = closed.find(endNode)
 
 # If we didn’t find a shorter route, skip.
 if endNodeRecord.costSoFar <= endNodeCost:
 continue
 
 # Otherwise remove it from the closed list.
 closed -= endNodeRecord
 
 # We can use the node’s old cost values to calculate
 # its heuristic without calling the possibly expensive
 # heuristic function.
 endNodeHeuristic = endNodeRecord.estimatedTotalCost -
 endNodeRecord.costSoFar
 
 # Skip if the node is open and we’ve not found a better
 # route.
 else if open.contains(endNode):
 # Here we find the record in the open list
 # corresponding to the endNode.
 
 endNodeRecord = open.find(endNode)
 
  # If our route is no better, then skip.
  if endNodeRecord.costSoFar <= endNodeCost:
  continue
 
  # Again, we can calculate its heuristic.
  endNodeHeuristic = endNodeRecord.cost -
  endNodeRecord.costSoFar
 
  # Otherwise we know we’ve got an unvisited node, so make a
  # record for it.
  else:
  endNodeRecord = new NodeRecord()
  endNodeRecord.node = endNode
 
  # We’ll need to calculate the heuristic value using
  # the function, since we don’t have an existing record
  # to use.
  endNodeHeuristic = heuristic.estimate(endNode)
 
  # We’re here if we need to update the node. Update the
  # cost, estimate and connection.
  endNodeRecord.cost = endNodeCost
  endNodeRecord.connection = connection
  endNodeRecord.estimatedTotalCost = endNodeCost +
 endNodeHeuristic
 
 # And add it to the open list.
 if not open.contains(endNode):
 open += endNodeRecord
 
 # We’ve finished looking at the connections for the current
 # node, so add it to the closed list and remove it from the
 # open list.
 open -= current
 closed += current
 
 # We’re here if we’ve either found the goal, or if we’ve no more
 # nodes to search, find which.
 if current.node != goal:
 # We’ve run out of nodes without finding the goal, so there’s
 # no solution.
 return null
 
 else:
 # Compile the list of connections in the path.
 
 path = []
 
 # Work back along the path, accumulating connections.
 while current.node != start:
 path += current.connection
 current = current.connection.getFromNode()
 
 # Reverse the path, and return it.
 return reverse(path)


```

### Wander (Minotauro)
```python
class KinematicWander :
  character: Static
  maxSpeed: float
  position: vector2
  # The maximum rotation speed we’d like, probably should be smaller
  # than the maximum possible, for a leisurely change in direction.
  maxRotation: float
 
  function getSteering() -> KinematicSteeringOutput:
   result = new KinematicSteeringOutput()
	
  newDir: vector2
	if(isIntersection(position) || randomBinomial(0, 10) < 3)
		toDir = randomDirection(position)

		newDir = toDir - position; 

		# Get velocity from the vector form of the orientation.
		result.velocity = maxSpeed * newDir
		
   return newDir;
```

## Seek(Minotauro)
```python
class KinematicSeek:
 character: Static
 target: Static

 maxSpeed: float

 function getSteering() -> KinematicSteeringOutput:
 result = new KinematicSteeringOutput()
 # Get the direction to the target.
 result.velocity = theseus.position - character.position

 # The velocity is along this direction, at full speed.
 result.velocity.normalize()
 result.velocity *= maxSpeed

 result.rotation = 0
 return result
```
#Estructura de Clases
![text](https://github.com/IAV22-G02/P1/blob/main/UML_Hamelin.png)

# Referencias Usadas:
+ AI for GAMES Third Edition, Ian Millintong
+ Unity 5.x Game AI Programming Cookbook, Jorge Palacios
