# Stratum : Game Design Document


# 0. Introducción
...


# 1. Reglamento
En esta sección se explicarán todas las mecánicas del juego. Al ser un juego de cartas, se seguirá la estructura que suelen tener los manuales de  juegos de cartas de mesa.

## 1.1. Descripción del juego
_Stratum_ es un juego de cartas para 4 jugadores, en el que hay un ecosistema al que contribuyen todos los jugadores jugando **cartas de población**: plantas, herbívoros y carnívoros. Este ecosistema está vivo y va cambiando si está desequilibrado, para balancearse de forma natural. Cada jugador juega el papel de un personaje. Los 4 personajes están divididos en 3 facciones, cada una de las cuales busca imponer sus intereses sobre las demás, usando las leyes de la naturaleza a su favor para equilibrar o desequilibrar el ecosistema. Además, cada jugador tiene en su mazo **cartas de influencia**, únicas para cada personaje, las cuales puede usar para influir en el ecosistema de manera artificial.

## 1.2. Facciones

### Naturaleza
Compuesta por los personajes **Sagitario** y **Ygdra**, esta facción gana la partida si, al final de una ronda, hay en la mesa al menos las siguientes cartas de población:
- 4 plantas
- 3 herbívoros
- 3 carnívoros

### Industria
Compuesta por el personaje **El Magnate**, esta facción gana la partida si, al final de una ronda, tiene construcciones en los 4 territorios.

### Fungi
Compuesta por el personaje **Fu'ngaloth**, esta facción gana la partida si, al final de una ronda, tiene 2 macrohongos en la mesa.

## 1.3. Inicio de la partida
Los jugadores se disponen, uno a cada lado de una mesa cuadrada, siguiendo este orden, en el sentido inverso a las agujas del reloj: Sagitario, Fu'ngaloth, Ygdra, El Magnate.

Cada jugador tiene delante su **territorio**. Este está compuesto por 5 espacios de territorio. Cuando sea su turno, el jugador podrá jugar cartas de criatura en estos espacios.

![Territorios](/Readme%20Files/tablero.png)

Cada jugador empieza con 5 cartas.

El ecosistema (la mesa) empieza con 4 cartas de población: 2 plantas, 1 herbívoro y 1 carnívoro. Estas se disponen de manera aleatoria, cada una en el espacio de territorio situado más a la izquierda de cada jugador.

Comienza jugando Sagitario, y el orden de turnos sigue el sentido inverso a las agujas del reloj.

## 1.4. Rondas y turnos
Una ronda consiste en los turnos de los 4 jugadores, y el turno del ecosistema.

Cada jugador en su turno tiene 2 acciones: una para jugar una **carta de población** y otra para jugar una **carta de influencia**. Puede elegir jugarlas en cualquier orden. 

Las cartas de población se pueden jugar sobre un espacio de territorio vacío. Cada jugador tiene que jugarlas en su territorio.

Cada carta de influencia explica cómo se debe jugar. Algunas se juegan poniéndolas sobre la mesa para aplicar su efecto y luego descartándolas, otras se ponen sobre cartas de población para concederles un efecto, ya sea inmediato o en el futuro, cuando se cumpla una condición.

El jugador puede usar una acción para descartar una carta, en vez de jugarla. También puede usar las 2 acciones y descartar 2 cartas. Si el jugador no puede jugar ninguna carta debe descartarse de 2 cartas, para siempre terminar su turno con 3 cartas en la mano.

Cuando ha terminado su turno el último jugador (Fu'ngaloth), es el turno del ecosistema. En este, las cartas de población que hay sobre la mesa pueden crecer o morir, según las Reglas del Ecosistema.

Cuando termina el turno del ecosistema, todos los jugadores roban 2 cartas, para volver a comenzar su turno con 5.


## 1.5. Ecosistema
En su turno, el ecosistema cambia según las cartas de población que haya sobre la mesa. Si las plantas, herbívoros y carnívoros no están en equilibrio, sucede al menos 1 de estos 2 casos:

- Una o más cartas de población **mueren**: la carta muerta se retira de la mesa, y si ninguna carta de influencia lo impide, se añade una carta de hongo en el espacio de territorio en el que estaba puesta dicha carta.

- Una o más cartas de población **crecen**: en el mismo espacio de territorio, encima de la carta de población que crece, se añade una carta igual.

La última carta de población de cada tipo (planta, herbívoro o carnívoro) en haberse jugado, se marca con ficha. Estas cartas son las que, en el turno del ecosistema, podrían crecer o morir si se cumplen las condiciones.

### Reglas del ecosistema
Estas son las directrices que se usan para determinar si hay cartas de población que crecen o mueren.

Las condiciones se comprueban secuencialmente (una por una), y los cálculos se hacen con el número de cartas resultantes después de haber realizado el cálculo de la condición anterior. Es decir, si empieza el turno del ecosistema habiendo 4 herbívoros, y después de comprobar la primera condición muere 1  herbívoro, las siguientes condiciones se comprobarán con 3 herbívoros, no con 4.

Como mucho puede morir o crecer 1 carta de herbívoro y 1 carta de carnívoro en cada turno del ecosistema.

Las condiciones, en orden de comprobación, son:

#### 1. Condiciones para que **mueran herbívoros**:
- Tiene que haber al menos una carta de herbívoro.
- Se debe cumplir al menos una de estas condiciones:
    - No hay cartas de planta en la mesa.
    - Debe haber al menos 2 cartas más de herbívoros que de plantas.
    - Debe haber al menos 2 cartas más de carnívoros que de herbívoros.

#### 2. Condiciones para que **crezcan herbívoros**:
- No puede haber muerto ninguna carta de herbívoro en la condición anterior.
- Tiene que haber al menos una carta de herbívoro.
- Tiene que haber al menos una carta de planta.
- Se debe cumplir al menos una de estas condiciones:
    - No hay cartas de carnívoro en la mesa.
    - Debe haber al menos 2 cartas más de herbívoros que de carnívoros.
    - Debe haber al menos 2 cartas más de plantas que de herbívoros.

#### 3. Condiciones para que **mueran carnívoros**:
- Tiene que haber al menos una carta de carnívoro.
- Se debe cumplir al menos una de estas condiciones:
    - No hay cartas de herbívoros en la mesa.
    - Debe haber al menos 2 cartas más de carnívoros que de herbívoros.

#### 4. Condiciones para que **crezcan carnívoros**:
- No puede haber muerto ningún carnívoro en la condición anterior.
- Tiene que haber al menos una carta de carnívoro.
- Debe haber al menos 2 cartas más de herbívoros que de carnívoros.

## 1.6. Personajes
Cada personaje tiene un objetivo único para ganar el juego, excepto los dos de la facción Naturaleza, que comparten el mismo objetivo. Los personajes de las facciones Industria y Fungi cuentan con mecánicas exclusivas para alcanzar sus objetivos y ganar la partida. Además, todos los personajes disponen de cartas de influencia propias.

### Ygdra
Es el ente protector de los árboles y plantas. Sus cartas de influencia están orientadas a las plantas.

Para ganar, debe, junto con Sagitario, conseguir que al final de una ronda, después del turno del ecosistema, haya en la mesa al menos 4 plantas, 3 herbívoros y 3 carnívoros.


### Sagitario
Es el ente protector de los animales. Sus cartas de influencia están orientadas a los animales.

Para ganar, debe, junto con Ygdra, conseguir que al final de una ronda, después del turno del ecosistema, haya en la mesa al menos 4 plantas, 3 herbívoros y 3 carnívoros.


### El Magnate
Representa la voluntad indomable de la humanidad, el deseo de imponerse a la naturaleza.

Para ganar debe conseguir que al final de una ronda, después del turno del ecosistema, haya construcciones en los 4 territorios.

#### Construcción
Es una acción exclusiva de El Magnate. En su turno, puede elegir gastar una acción para construir sobre uno de los 4 territorios. Para poder construir sobre un territorio se deben cumplir las siguientes condiciones:
- El territorio no tiene una construcción.
- El territorio no tiene ninguna carta de población de carnívoros.
- El territorio no tiene ninguna carta de población con una  carta de influencia puesta que impida construir.
- El territorio tiene al menos 2 cartas de población de plantas.

Al construir, se pone una ficha en frente del territorio para indicar que tiene construcción. Mueren las 2 cartas de planta más recientes del territorio.

Después, se puede jugar con normalidad cartas de criatura sobre el territorio construido, y la construcción se puede destruir usando cartas de influencia que lo permitan.

Se puede construir nuevamente sobre un territorio que tuvo un terreno y fue destruido, si se cumplen las condiciones descritas.

### Fu'ngaloth
Es una deidad ancestral de los hongos, cuya voluntad es que los hongos se impongan sobre todas las otras formas de vida.

Para ganar debe conseguir que al final de una ronda, después del turno del ecosistema, haya en la mesa al menos 2 macrohongos.

#### Macrohongo
Es una acción exclusiva de Fu'ngaloth. En su turno, puede elegir gastar una acción para crear un macrohongo. Para poder crearlo, debe haber al menos 3 cartas de hongo en la mesa. La acción consiste en descartar 3 cartas de hongo, y poner una carta de macrohongo en uno de esos 3 espacios de territorio, a elección del jugador.

El macrohongo no puede ser destruido por cartas de influencia, a menos que la descripción de la carta lo indique explícitamente.

## 1.7. Cartas
### Cartas de población
- **Planta**
- **Herbívoro**
- **Carnívoro**

### Cartas de influencia
#### Ygdra
- **Incendio Forestal:** Elige un territorio. Todas las cartas de población, hongo y macrohongo en ese territorio mueren. Si hay una construcción, también se destruye.
- **Fruta con semillas:** Coloca esta carta sobre una carta de planta. Si una carta de herbívoro o carnívoro de ese territorio crece, otra planta crecerá sobre la que tiene la carta.
- **Mala hierba nunca muere:** Coloca esta carta sobre una carta de planta. Si muere, crece otra carta de planta igual y esta carta se descarta.
- **Fragancia de feromonas:** Elige una carta de herbívoro o carnívoro de otro territorio y muévela a un espacio vacío en tu territorio.
- **Hiedra Verde:** Coloca esta carta sobre una carta de planta. Si la carta de planta permanece una ronda completa en un territorio con construcción, la construcción se destruye y esta carta se descarta.

#### Sagitario
- **Hibernación:** Coloca esta carta sobre una carta de herbívoro. Esta carta no podrá morir ni crecer por las reglas del ecosistema.
- **Rabia:** Coloca esta carta sobre una carta de herbívoro. Mientras no muera, El Magnate no podrá construir en el territorio donde esté.
- **Migración:** Elige una carta de carnívoro o herbívoro de tu territorio y muévela a un espacio vacío en otro territorio.
- **Omnívoro:** Coloca esta carta sobre una carta de carnívoro. A partir de ahora, esta carta contará tanto como carnívoro como herbívoro.
- **Depredador de setas:** Coloca esta carta sobre una carta de herbívoro. Mientras no muera, al final de la ronda morirá la carta de hongo más reciente que haya en su territorio.



#### El Magnate
- **Incendio provocado:** Elige un territorio. Todas las cartas de población, hongo y macrohongo en ese territorio mueren. Si hay una construcción, también se destruye.
- **Pesticida:** Muere una carta de planta de tu elección.
- **Cazador:** Muere una carta de herbívoro o carnívoro de tu elección.
- **Fuegos artificiales:** Elige una carta de herbívoro o carnívoro y muévela a un espacio vacío en el territorio opuesto al que se encuentra.
- **Compost:** Elige un espacio vacío de un territorio. Coloca una carta de hongo y una carta de planta encima.

#### Fu'ngaloth
- **Esporas explosivas:** Elige un territorio donde haya al menos una carta de hongo o macrohongo. Todas las cartas de población en ese territorio mueren. Si hay una construcción también se destruye. Las cartas de hongo y macrohongo no mueren.
- **Putrefacción:** Muere una carta de planta de tu elección. Crece un hongo en su espacio de territorio.
- **Parásito:** Coloca esta carta sobre una carta de herbívoro o carnívoro. Si la carta crece, aparece un hongo en su espacio de territorio y esta carta se descarta.
- **Seta apetitosa:** Elige una carta de herbívoro o carnívoro y muévela a un espacio vacío en un territorio donde haya al menos una carta de hongo o macrohongo.
- **Moho:** Coloca una carta de hongo sobre un espacio vacío de un territorio con construcción.


# 2. Controles
...


# 3. Narrativa [PROVISIONAL]

### **Historia del Juego**

En un rincón olvidado del mundo, oculto del alcance de la humanidad, yace el legendario bosque de **Sylveria**, un lugar antiguo y sagrado donde la vida fluye en armonía. Este bosque es el centro de un conflicto ancestral entre tres facciones primordiales: **Naturaleza**, **Fungi**, y la reciente aparición de la **Industria**. Cada una busca imponer su visión sobre el destino de **Sylveria**, pues controlar este bosque es controlar la fuente de toda su vitalidad.

El equilibrio del bosque está determinado por un juego antiguo, conocido como **Stratum**, cuyas reglas han sido esculpidas por el tiempo y la magia que envuelve **Sylveria**. Desde la creación del bosque, el poder y la vida en este lugar han sido regidos por este misterioso juego de cartas, y ahora, como jugador, es tu turno de decidir quién tomará el control de **Sylveria**.

#### **Sylveria: El Corazón del Bosque**

**Sylveria** es mucho más que un simple bosque; es el núcleo de toda la vida natural, y su influencia se extiende a todas las criaturas y plantas que residen dentro de sus límites. Sus árboles milenarios, cuyas raíces se adentran profundamente en la tierra, conectan con la esencia misma del mundo, sirviendo de refugio para las energías naturales que sostienen el equilibrio.

Cada facción tiene sus propios motivos para desear el control de **Sylveria**. Para la **Naturaleza**, es su santuario más preciado; para la **Industria**, una tierra rica en recursos que debe ser conquistada; y para los **Fungi**, es el lugar perfecto para extender su dominio micelial. **Stratum** es el medio por el cual este control será disputado, y quien gane dominará el destino del bosque y su futuro.

#### **Naturaleza**

La facción de la **Naturaleza** es salvaguardada por dos guardianes primordiales: **Sagitario**, protector de los animales, e **Ygdra**, la guardiana de las plantas. Ellos han mantenido a **Sylveria** en equilibrio durante generaciones, asegurando que los ecosistemas dentro del bosque prosperen sin perturbaciones. Los ríos cristalinos y los densos bosques que definen este paisaje son testigos de su dedicación.

Los guardianes de la **Naturaleza** enfrentan amenazas constantes. Por un lado, las oscuras fuerzas de los **Fungi** buscan invadir y corromper **Sylveria**, transformando sus tierras fértiles en un reino fúngico. Por otro, la **Industria** avanza con sus máquinas, talando árboles y construyendo estructuras que contaminan el aire y la tierra. Los protectores naturales están decididos a evitar la destrucción del bosque, sabiendo que si **Sylveria** cae, el equilibrio de la vida en este lugar se romperá para siempre.

#### **Industria**

La **Industria** ha surgido con el implacable avance humano, liderada por el ambicioso **Magnate**. Su visión es clara: transformar el bosque de **Sylveria** en una fuente de recursos y progreso. El avance de la **Industria** no se detiene ante nada, pues las verdes tierras son vistas como materiales en bruto, listos para ser explotados. Bajo su mando, el gris cemento y el frío metal han comenzado a reemplazar los árboles y ríos que una vez prosperaban aquí.

El **Magnate** ve a **Sylveria** como la joya que falta para completar su dominio. Cada fábrica que construye y cada bosque que arrasa lo acerca a su meta de controlar el último reducto de resistencia natural. **Stratum** es el camino para lograrlo, y bajo las reglas de este juego ancestral, la **Industria** hará lo que sea necesario para someter el bosque a su voluntad.

#### **Fungi**

En lo más profundo de la tierra, **Fu'ngaloth**, la antigua deidad de los hongos, espera pacientemente. Su reino fúngico crece bajo la superficie de **Sylveria**, y aunque no se ve a simple vista, su influencia se extiende lentamente por el bosque. Los **Fungi** no buscan una conquista rápida; su estrategia es más sigilosa. Propagan sus esporas y sus macrohongos con el fin de asimilar todo lo que tocan, y en **Sylveria** encuentran el terreno fértil que necesitan para expandirse.

El poder de **Fu'ngaloth** reside en su capacidad para corromper la vida, transformando lo verde en descomposición, y conectando todo lo que ha tocado a su vasto reino micelial. Para él, **Sylveria** no es solo un lugar, sino el portal a la dominación total de la vida que habita en sus tierras. El control del bosque asegurará su expansión sin fin.

#### **Conflicto**

El conflicto en **Sylveria** nace de la colisión de estas tres poderosas facciones. Cada una de ellas tiene sus propias ambiciones y motivos para desear el control del bosque, pero todas entienden que, sin **Sylveria**, sus planes están condenados al fracaso. Mientras que la **Naturaleza** busca mantener el equilibrio, tanto la **Industria** como los **Fungi** luchan por someter al bosque y dominar sus riquezas.

Aunque las facciones de **Industria** y **Fungi** podrían aliarse temporalmente para desestabilizar a la **Naturaleza**, sus ambiciones individuales son irreconciliables. El **Magnate** ansía someter el bosque a su maquinaria, mientras **Fu'ngaloth** desea absorber **Sylveria** en su red de hongos. La traición está asegurada, ya que ninguno de los dos está dispuesto a compartir el poder que este lugar otorga.

Como jugador, ahora es tu turno. **Stratum**, el juego ancestral que ha determinado el destino del bosque desde tiempos inmemoriales, está en tus manos. Navegarás esta compleja red de alianzas temporales y traiciones inevitables mientras luchas por controlar **Sylveria**. ¿Te aliarás con otros para lograr tus objetivos, o caerás víctima de las traiciones en esta batalla por el control del bosque?


# 4. Arte y apartado visual
...


# 5. Sonido y Música
...


# 6. Modelo de negocio y monetización
