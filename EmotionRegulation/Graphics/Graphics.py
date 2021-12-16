
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np


path0= "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset/Results/NormalEvents.xlsx"
path1= "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset/Results/Conscientiousness.xlsx"
path2= "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset/Results/Extraversion.xlsx"
path3= "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset/Results/Neuroticism.xlsx"
path4= "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset/Results/Openness.xlsx"
path5= "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset/Results/Agreeableness.xlsx"

df_WithoutRegulation  = pd.read_excel(path0)
df_EmotionRegulation1 = pd.read_excel(path1)

df_EmotionRegulation2 = pd.read_excel(path2)
df_EmotionRegulation3 = pd.read_excel(path3)
df_EmotionRegulation4 = pd.read_excel(path4)
df_EmotionRegulation5 = pd.read_excel(path5)


MOOD0 = (df_WithoutRegulation["MOOD     "].dropna())
EMOTION0 = (df_WithoutRegulation["EMOTION  "].dropna())
INTENSITY0 = (df_WithoutRegulation["INTENSITY"].dropna())
EVENT0 = (df_WithoutRegulation["   EVENT    "].dropna())
STRATEGY0 = (df_WithoutRegulation[" APPLIED STRATEGY    "].dropna())
PERSONALITIES0 = (df_WithoutRegulation[" PERSONALITY TRAITS "].dropna())
SRELATED0 = (df_WithoutRegulation[" STRATEGIES RELATED "].dropna())
PERSONALITY0 = (df_WithoutRegulation[" DOMINANT PERSONALITY "].dropna())

MOOD1 = (df_EmotionRegulation1["MOOD     "].dropna())
EMOTION1 = (df_EmotionRegulation1["EMOTION  "].dropna())
INTENSITY1 = (df_EmotionRegulation1["INTENSITY"].dropna())
EVENT1 = (df_EmotionRegulation1["   EVENT    "].dropna())
STRATEGY1 = (df_EmotionRegulation1[" APPLIED STRATEGY    "].dropna())
PERSONALITIES1 = (df_EmotionRegulation1[" PERSONALITY TRAITS "].dropna())
SRELATED1 = (df_EmotionRegulation1[" STRATEGIES RELATED "].dropna())
PERSONALITY1 = (df_EmotionRegulation1[" DOMINANT PERSONALITY "].dropna())

MOOD2 = (df_EmotionRegulation2["MOOD     "].dropna())
EMOTION2 = (df_EmotionRegulation2["EMOTION  "].dropna())
INTENSITY2 = (df_EmotionRegulation2["INTENSITY"].dropna())
EVENT2 = (df_EmotionRegulation2["   EVENT    "].dropna())
STRATEGY2 = (df_EmotionRegulation2[" APPLIED STRATEGY    "].dropna())
PERSONALITIES2 = (df_EmotionRegulation2[" PERSONALITY TRAITS "].dropna())
SRELATED2 = (df_EmotionRegulation2[" STRATEGIES RELATED "].dropna())
PERSONALITY2 = (df_EmotionRegulation2[" DOMINANT PERSONALITY "].dropna())

MOOD3 = (df_EmotionRegulation3["MOOD     "].dropna())
EMOTION3 = (df_EmotionRegulation3["EMOTION  "].dropna())
INTENSITY3 = (df_EmotionRegulation3["INTENSITY"].dropna())
EVENT3 = (df_EmotionRegulation3["   EVENT    "].dropna())
STRATEGY3 = (df_EmotionRegulation3[" APPLIED STRATEGY    "].dropna())
PERSONALITIES3 = (df_EmotionRegulation3[" PERSONALITY TRAITS "].dropna())
SRELATED3 = (df_EmotionRegulation3[" STRATEGIES RELATED "].dropna())
PERSONALITY3 = (df_EmotionRegulation3[" DOMINANT PERSONALITY "].dropna())

MOOD4 = (df_EmotionRegulation4["MOOD     "].dropna())
EMOTION4 = (df_EmotionRegulation4["EMOTION  "].dropna())
INTENSITY4 = (df_EmotionRegulation4["INTENSITY"].dropna())
EVENT4 = (df_EmotionRegulation4["   EVENT    "].dropna())
STRATEGY4 = (df_EmotionRegulation4[" APPLIED STRATEGY    "].dropna())
PERSONALITIES4 = (df_EmotionRegulation4[" PERSONALITY TRAITS "].dropna())
SRELATED4 = (df_EmotionRegulation4[" STRATEGIES RELATED "].dropna())
PERSONALITY4 = (df_EmotionRegulation4[" DOMINANT PERSONALITY "].dropna())

MOOD5 = (df_EmotionRegulation5["MOOD     "].dropna())
EMOTION5 = (df_EmotionRegulation5["EMOTION  "].dropna())
INTENSITY5 = (df_EmotionRegulation5["INTENSITY"].dropna())
EVENT5 = (df_EmotionRegulation5["   EVENT    "].dropna())
STRATEGY5 = (df_EmotionRegulation5[" APPLIED STRATEGY    "].dropna())
PERSONALITIES5 = (df_EmotionRegulation5[" PERSONALITY TRAITS "].dropna())
SRELATED5 = (df_EmotionRegulation5[" STRATEGIES RELATED "].dropna())
PERSONALITY5 = (df_EmotionRegulation5[" DOMINANT PERSONALITY "].dropna())

personalities = PERSONALITIES1.str.cat(sep=', ')
Dominant = PERSONALITY1.str.cat().upper()

events = EVENT0
Strategy = STRATEGY1.str.cat(sep=' -- ')
strategyLabel = "--strategy applied--"

plt.rcParams["figure.figsize"] = (15, 10)

plt.plot(MOOD0, marker="o",label='NOT-Regulation')
plt.plot(MOOD1,'o--', label=PERSONALITY1.str.cat().upper())
plt.plot(MOOD2,'o--', label=PERSONALITY2.str.cat().upper())
plt.plot(MOOD3,'o--', label=PERSONALITY3.str.cat().upper())
plt.plot(MOOD4,'o--', label=PERSONALITY4.str.cat().upper())
plt.plot(MOOD5,'o--', label=PERSONALITY5.str.cat().upper())

plt.xticks(rotation=15)
plt.xticks(np.arange(len(events)),"   " + events )
plt.title("EMOTION REGULATION MODEL")
#plt.title("Character 1: Dominant Personality : "+Dominant +"\n TRAITS : "+ personalities)
plt.legend(loc='best')
plt.xlabel("\n--" + Strategy+ "-- ")
plt.ylabel("Mood value")
plt.grid()
plt.savefig("Ejemplo1.jpg")
plt.show()

x = MOOD1
y = INTENSITY0
y2 = INTENSITY1
numero_de_grupos = len(EMOTION0)
indice_barras = np.arange(numero_de_grupos)
ancho_barras =0.3
plt.bar(indice_barras, y2, width=ancho_barras,  label="REGULATION MODEL")
plt.bar(indice_barras + ancho_barras, y, width=ancho_barras, label='NOT-Regulation')
plt.legend(loc='best')
plt.xticks(indice_barras + ancho_barras, EMOTION0 +"\n\n" + "--"+ events+"--")
plt.xticks(rotation=15)
plt.title("Character 1: Dominant Personality : "+Dominant +"\n TRAITS : "+ personalities)
plt.ylabel('Intensity')
plt.xlabel("Emotions\n --Events-- ")
plt.savefig("Ejemplo2.jpg")
plt.show()



