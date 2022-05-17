# Snake

## Consignes

Reproduire la build fournie à l’aide des instructions ci-dessous. Il est attendu une build et le projet mis sur Github. Bon courage !

## Préambule

Le jeu se jouant sur une grille, vous devez arrondir tous les vecteurs afin de n’avoir que des nombres entiers pour les positions et directions. Ceci se fait à l’aide de la *structure* [`Vector2Int`](https://docs.unity3d.com/ScriptReference/Vector2Int.html).

### Vector2Int

Un **`Vector2Int`** se crée comme un `Vector2` mais n’accepte que des `int` comme paramètres.

```C#
var position = new Vector2Int(4, 8); // Code valide
var error = new Vector2Int(4.5f, 8.6f); // Code invalide
```
Les **`Vector2Int`** se comportent comme des `Vector2` dans toutes les opérations (addition, soustraction, multiplication, normalisation, calcul de la magnitude etc.).
Il est possible de convertir un `Vector2` en **`Vector2Int`** en arrondissant le vecteur.

### Arrondis

Il existe 3 arrondis possible pour passer d’un `Vector2` à **`Vector2Int`**.

* [`RoundToInt()`](https://docs.unity3d.com/ScriptReference/Vector2Int.RoundToInt.html) : Arrondit indépendamment chaque composant du `Vector2` à l’entier le plus proche.
* [`CeilToInt()`](https://docs.unity3d.com/ScriptReference/Vector2Int.CeilToInt.html) : Arrondit chaque composant à l’entier supérieur.
* [`FloorToInt()`](https://docs.unity3d.com/ScriptReference/Vector2Int.FloorToInt.html) : Arrondit chaque composant à l’entier inférieur.

```C#
var floatPosition = new Vector2(4.3f, 5.8f);
var roundPosition = Vector2Int.RoundToInt(floatPosition);
var ceilPosition = Vector2Int.CeilToInt(floatPosition);
var floorPosition = Vector2Int.FloorToInt(floatPosition);

Debug.Log(roundPosition); // Affiche (4, 6)
Debug.Log(ceilPosition); // Affiche (5, 6)
Debug.Log(floorPosition); // Affiche (4, 5)
```
### Limitations

Les **`Vector2Int`** ne **peuvent pas** être utilisés pour instancier des objets et modifier des positions dans un `Transform`. Ainsi les codes suivants ne compileront pas !

```C#
var intPosition = new Vector2Int(5, 5);
Instantiate(gameObject, intPosition, Quaternion.identity);
```
```C#
var intPosition = new Vector2Int(5, 5);
transform.position = intPosition;
```
Cela est dû au fait qu’il n’existe pas de conversion automatique de **`Vector2Int`** vers `Vector3` alors qu’il en existe une de `Vector2` vers `Vector3`. Dans ces cas là, reconvertissez votre **`Vector2Int`** en `Vector2` comme ceci : 
```C#
var intPosition = new Vector2Int(5, 5);
var floatPosition = (Vector2)intPosition; // floatPosition est un Vector2
```

## Déplacements de la tête (SnakeController)

1) Dans la fonction `OnMove()`, stoppez la fonction si l’input ne vient pas d’être appuyé. Ensuite récupérez la valeur de l’input sous la forme d’un `Vector2` puis convertissez la en `Vector2Int`. Cette valeur sera appelée `currentInput`.

 2) Créez un booléen privé appelé `isAlive` qui vaut **vrai** par défaut. Créez une coroutine qui se lance au démarrage du jeu et qui s’appelle `Move`. Cette coroutine doit s’exécuter toutes les `timeBeforeMove` secondes tant que `isAlive` est vrai. La valeur `timeBeforeMove` doit être modifiable dans l’inspecteur. 
 
3) À chaque exécution de `Move`, récupérez dans une variable appelée `emptySpace` la position actuelle du `RigidBody2D` de l’objet.

4) Ensuite, toujours dans `Move`, utilisez la fonction [`MovePosition()`](https://docs.unity3d.com/2021.2/Documentation/ScriptReference/Rigidbody2D.MovePosition.html) du `RigidBody2D` afin de téléporter la tête du serpent dans la direction de `currentInput`. Par exemple si la position du `RigidBody2D` est `(12, 24)` et que `currentInput` est `(1, 0)`, le `RigidBody2D` doit se téléporter en `(13, 24)`.

5) !!! Faites en sorte que le serpent ne puisse pas revenir en arrière. C’est à dire que si le serpent s’est déplacé vers le haut lors du dernier `Move`, à savoir que son ancien input est `(0, 1)`, il ne peut au déplacement suivant avoir un `currentInput` de `(0, -1)`, sinon il reviendrait sur ses pas.

6) !!! Unity permet de transformer n’importe quel *message* en coroutine. Ainsi les deux exemples suivants sont valides mais le deuxième nous permet d’attendre un temps indéterminé au milieu de la fonction.
	```C#
	private void Start()
	{
	}
	```

	```C#
	private IEnumerator Start()
	{
	}
	```
	Également, il est possible d’attendre tant qu’une valeur a une certaine valeur dans une coroutine à l’aide de `WaitWhile()`. Ainsi dans l’exemple suivant la fonction attendra que 15 secondes de jeu soit passées avant de continuer.
	```C#
	private void Start()
	{
		StartCoroutine(DebugWhenSecondsHasPassed());
	}
	
	private IEnumerator DebugWhenSecondsHasPassed()
	{
		yield return new WaitWhile(() => Time.time < 15.0f);
		Debug.Log("15 secondes sont passées depuis le lancement");
	}
	```
	À partir de ces informations, ne lancez la Coroutine `Move` qu’après que `currentInput` ait été modifié une fois.

**Résultat attendu :**

[GIF](https://i.imgur.com/Fs7Vr2e.gif)

## Gestion de la queue (SnakeController)

1) Au début du `Start()`, créez un objet vide dans la hiérarchie appelé "Tail" et enregistrez son `Transform` dans une variable nommée `tail`. Pour créer un objet vide, plutôt que d’utiliser un `Instantiate()`, vous pouvez faire `new GameObject("Nom de l’objet")` (cf. [documentation](https://docs.unity3d.com/ScriptReference/GameObject-ctor.html)).

2) Juste après la création de `tail`, instanciez un bout de queue en enfant de `tail` à la position `(1, 0, 0)`. Le prefab s’appelle *TailPart* et se situe dans le dossier *Assets/Prefabs*.

3) Dans la coroutine `Move`, on doit attendre que la téléportation de la tête soit finie avant de déplacer la queue pour éviter de collisionner avec nous même par accident. Ainsi, parmi les lignes d’exemple suivantes, ajoutez celle qui permet d’attendre la mise à jour du moteur physique juste après la téléportation.

```C#
yield return null;
yield return new WaitForEndOfFrame();
yield return new WaitForFixedUpdate();
yield return new WaitForSeconds(Physics.timeStep);
yield return new WaitForSecondsRealtime(0.02f);
yield return new WaitWhile(() => true);
```

4) Juste après avoir attendu, il nous faut téléporter chaque bout de queue à leur nouvelle position. Or chaque bout de queue doit occuper l’ancien emplacement du bout de queue présent devant lui. Ainsi, par exemple, le bout de queue 3 prend l’ancienne place du bout 2 qui prend l’ancienne place du bout 1 qui prend l’ancienne place de la tête. Ce comportement doit être implémenté grâce à l’algorithme suivant :
Pour chacun des enfants de `tail`:
	* Récupérez la position actuelle de l’enfant dans une variable appelée `lastPosition`.
	* Téléportez l’enfant à la position d’`emptySpace`.
	* Assignez `lastPosition` à `emptySpace`.

**Résultat attendu :**
[GIF](https://i.imgur.com/IZAAzFh.gif)

## Apparition des fruits (Spawner)

1) Ajoutez une variable publique de type `BoxCollider2D` afin d’y drag & drop l’objet *PlayZone* présent dans la scène. Ce collider représente la zone de jeu, à savoir la zone dans laquelle le serpent a le droit de naviguer mais également la zone dans laquelle un fruit a le droit de spawner.

2) !!! Au lancement du `Spawner`, il nous faut calculer les positions avec les composants les plus élevés et les plus bas contenus dans le collider de *PlayZone* afin de générer une valeur aléatoire entre ces deux extrêmes. Pour cela on peut accéder aux variables [`min`](https://docs.unity3d.com/ScriptReference/Bounds-min.html) et [`max`](https://docs.unity3d.com/ScriptReference/Bounds-max.html) présentes dans des [`bounds`](https://docs.unity3d.com/ScriptReference/Collider2D-bounds.html) elles même présentes dans les [`Collider2D`](https://docs.unity3d.com/ScriptReference/Collider2D.html). Enregistrez le `max` des `bounds` du `BoxCollider2D` présent sur *PlayZone* dans une variable nommée `tempMaxSpawnPos`. Faites la même chose avec le `min` dans une variable `tempMinSpawnPos`. 

3) !!! Afin de s’assurer que nos positions soit des entiers dans la *PlayZone*, arrondissez à l’inférieur `tempMaxSpawnPos` dans une variable nommée `maxSpawnPos`. Puis arrondissez au supérieur `tempMinSpawnPos` dans une variable nommée `maxSpawnPos`.

4) Si vous n’arrivez pas aux deux questions précédentes, créez juste deux `Vector2Int` privés avec les valeurs suivantes : `minSpawnPos` vaut `(-17, -9)` et `maxSpawnPos` vaut `(17, 9)`.
 
 5) Créez une fonction publique appelée `SpawnFruit()`. Dans celle ci calculez des valeurs aléatoire pour la position en X et Y de l’objet puis instanciez un *Fruit* à la position aléatoire. Les valeurs aléatoires doivent être comprises entre `minSpawnPos` et `maxSpawnPos`. Le prefab à instancier s’appelle *Fruit* et se situe dans le dossier *Assets/Prefabs*.
 
6) Lancez la fonction `SpawnFruit()` au démarrage du script après le calcul des positions minimale et maximale.

**Résultat attendu :**
[GIF](https://i.imgur.com/mm2SLbt.gif)

##  Agrandissement du serpent (SnakeController)

1) Après le déplacement des bouts de queue, il nous faut faire apparaître un nouveau bout de queue si on a mangé un fruit. Commencez par créer une fonction `AddTailPart` qui prend en paramètre un `Vector2` appelé `newPartPosition` et un booléen privé appelé `fruitEaten` qui vaut **vrai** au lancement du jeu.

2) Dans `AddTailPart`, arrêtez la fonction au début si `fruitEaten` est faux. Sinon instanciez un bout de queue en enfant de `tail` à la position `newPartPosition` puis passez `fruitEaten` à faux.

3) Supprimez l’instanciation du bout de queue dans le `Start` et appelez `AddTailPart` dans la coroutine `Move` après le déplacement de queue avec `emptySpace` comme paramètre. Cela aura pour effet de faire apparaître un bout de queue à la fin de celle-ci au prochain déplacement.

4) Créez une variable privée qui comptera le nombre de fruit avalé appelée `eatenFruits`.

5) Créez une fonction publique `EatFruit` qui passe la variable `fruitEaten` à vrai et incrémente `eatenFruits`.

**Résultat attendu :**
[GIF](https://i.imgur.com/Pe8cy03.gif)

## Comportement des fruits (Fruit)

1) Créez une variable privée de type `Spawner` appelée `spawner` puis au `Start`, assignez lui le script `Spawner` présent sur l’objet *Fruit Spawner*.

2) !!! Toujours dans le `Start`, récupérez le `SpriteRenderer` présent sur l’objet et assignez à sa [couleur](https://docs.unity3d.com/ScriptReference/SpriteRenderer-color.html) une couleur aléatoire. Pour ça, utilisez la classe [`Random`](https://docs.unity3d.com/ScriptReference/Random.html). Pour finir faites en sorte que la couleur aléatoire possède une `hue` entre 0 et 1, une `saturation` entre 0 et 1, une `value` à 1 et une `alpha` à 1.

3) Lorsque le fruit est trigger par un objet, si l’objet qui collisionne avec possède un script `SnakeController`,  appelez la fonction `EatFruit` sur le `SnakeController` qui vous a trigger, appelez la fonction `SpawnFruit` dans `spawner` puis détruisez le fruit.

**Résultat attendu :**
[GIF](https://i.imgur.com/lG8QI6l.gif)

## Bouton de restart (RestartButton)

1) Dans la fonction `RestartScene`, rechargez la scène active. 

## Affichage du score (ScoreDisplayer)

1) Créez une variable privée contenant un *TextMeshPro* appelée `tmPro` puis assignez le component *TextMeshPro* à `tmPro` dans un `Awake`.

2) Dans `SetScore`, affichez dans le texte de `tmPro` la valeur `score` passée en paramètre.

## Mort (SnakeController)

1) Créez une fonction `Die`. Cette fonction doit s’arrêter immédiatement si `isAlive` est faux. Dans cette fonction il faut :
	* Passer `isAlive` à faux
	* Instancier l’écran de Game Over. Le prefab s’appelle *GameOverCanvas* et se situe dans le dossier *Assets/Prefabs*.
	* Sur cet écran de Game Over, trouver le script `ScoreDisplayer` et appeler dessus la fonction `SetScore` avec la variable `eatenFruits` en paramètres.
	* Détruire la queue.
	* Détruire l’objet sur lequel on est.

2) Si le serpent sort d’un *trigger* taggé "PlayZone", appelez `Die`.

3) Si le serpent entre dans un trigger qui est en enfant de `tail`, appelez `Die`.


**Résultat attendu :**
La build que vous avez.
